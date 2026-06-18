using FirinApi.Models.Common;
using FirinApi.Models.DTOs.Orders;
using FirinApi.Models.Entities;
using FirinApi.Models.Enums;
using FirinApi.Repositories.Interfaces;
using FirinApi.Services.Interfaces;

namespace FirinApi.Services.Implementations;

/// <summary>
/// Sipariş iş kuralları burada yönetilir:
/// sipariş oluşturma, durum güncelleme ve iptal (stok iadesi).
/// </summary>
public class OrderService(
    IOrderRepository orderRepository,
    ICustomerRepository customerRepository,
    IBreadRepository breadRepository,
    ICakeRepository cakeRepository) : IOrderService
{
    public async Task<PagedResult<OrderDto>> GetPagedAsync(PaginationQuery query, CancellationToken cancellationToken = default)
    {
        query.Normalize();
        var (orders, totalCount) = await orderRepository.GetPagedWithDetailsAsync(query.Skip, query.Size, cancellationToken);
        return new PagedResult<OrderDto>
        {
            Items = orders.Select(MapToDto).ToList(),
            Page = query.Page,
            PageSize = query.Size,
            TotalCount = totalCount
        };
    }

    public async Task<OrderDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        return order is null ? null : MapToDto(order);
    }

    public async Task<OrderDto> CreateAsync(CreateOrderDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.Items.Count == 0)
            throw new InvalidOperationException("Sipariş en az bir ürün içermelidir.");

        // Müşterinin gerçekten var olduğunu kontrol et
        var customer = await customerRepository.GetByIdAsync(dto.CustomerId, cancellationToken)
            ?? throw new InvalidOperationException("Müşteri bulunamadı.");

        var order = new Order
        {
            CustomerId = customer.Id,
            Status = OrderStatus.Pending // Yeni sipariş "Beklemede" başlar
        };

        // Her sipariş kalemi için ürünü bul, fiyatı al ve ekmekse stoktan düş
        foreach (var itemDto in dto.Items)
        {
            var (productName, unitPrice) = await ResolveProductAsync(itemDto, cancellationToken);

            order.Items.Add(new OrderItem
            {
                ProductType = itemDto.ProductType,
                ProductId = itemDto.ProductId,
                ProductName = productName,
                Quantity = itemDto.Quantity,
                UnitPrice = unitPrice
            });

            order.TotalAmount += unitPrice * itemDto.Quantity;
        }

        await orderRepository.AddAsync(order, cancellationToken);
        await orderRepository.SaveChangesAsync(cancellationToken);

        order.Customer = customer;
        return MapToDto(order);
    }

    public async Task<OrderDto?> UpdateStatusAsync(int id, UpdateOrderStatusDto dto, CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        if (order is null) return null;

        // Durum "İptal" olarak değiştiriliyorsa stok iadesi yap
        if (dto.Status == OrderStatus.Cancelled && order.Status != OrderStatus.Cancelled)
        {
            EnsureOrderCanBeCancelled(order);
            await RestoreStockAsync(order, cancellationToken);
        }

        order.Status = dto.Status;
        orderRepository.Update(order);
        await orderRepository.SaveChangesAsync(cancellationToken);
        return MapToDto(order);
    }

    /// <summary>
    /// Siparişi iptal eder ve ekmek stoklarını rafa geri koyar.
    /// </summary>
    public async Task<OrderDto?> CancelAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        if (order is null) return null;

        // Zaten iptal edilmiş veya tamamlanmış sipariş kontrolü
        EnsureOrderCanBeCancelled(order);

        // Ekmek stoklarını geri iade et
        await RestoreStockAsync(order, cancellationToken);

        order.Status = OrderStatus.Cancelled;
        orderRepository.Update(order);
        await orderRepository.SaveChangesAsync(cancellationToken);
        return MapToDto(order);
    }

    /// <summary>
    /// İptal kuralları: sadece Beklemede veya Onaylanmış siparişler iptal edilebilir.
    /// </summary>
    private static void EnsureOrderCanBeCancelled(Order order)
    {
        if (order.Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Sipariş zaten iptal edilmiş.");

        if (order.Status == OrderStatus.Completed)
            throw new InvalidOperationException("Tamamlanmış sipariş iptal edilemez.");
    }

    /// <summary>
    /// Siparişteki ekmekleri stoğa geri ekler.
    /// Örnek: 5 ekmek sipariş edilmiş ve iptal edildi → stoka +5 eklenir.
    /// Not: Pastaların stok takibi yok, sadece ekmekler iade edilir.
    /// </summary>
    private async Task RestoreStockAsync(Order order, CancellationToken cancellationToken)
    {
        foreach (var item in order.Items)
        {
            // Sadece ekmek tipindeki ürünlerin stoğu geri verilir
            if (item.ProductType != ProductType.Bread)
                continue;

            var bread = await breadRepository.GetByIdAsync(item.ProductId, cancellationToken);
            if (bread is null)
                continue; // Ürün silinmişse atla, iptali engelleme

            bread.StockQuantity += item.Quantity; // Stoka geri ekle
            breadRepository.Update(bread);
        }
    }

    private async Task<(string Name, decimal Price)> ResolveProductAsync(
        CreateOrderItemDto item,
        CancellationToken cancellationToken)
    {
        if (item.Quantity <= 0)
            throw new InvalidOperationException("Ürün adedi 0'dan büyük olmalıdır.");

        return item.ProductType switch
        {
            ProductType.Bread => await ResolveBreadAsync(item.ProductId, item.Quantity, cancellationToken),
            ProductType.Cake => await ResolveCakeAsync(item.ProductId, cancellationToken),
            _ => throw new InvalidOperationException("Geçersiz ürün tipi.")
        };
    }

    /// <summary>
    /// Ekmek siparişi: stok kontrolü yap ve stoktan düş.
    /// </summary>
    private async Task<(string Name, decimal Price)> ResolveBreadAsync(
        int productId,
        int quantity,
        CancellationToken cancellationToken)
    {
        var bread = await breadRepository.GetByIdAsync(productId, cancellationToken)
            ?? throw new InvalidOperationException($"Ekmek bulunamadı (Id: {productId}).");

        if (bread.StockQuantity < quantity)
            throw new InvalidOperationException($"'{bread.Name}' için yeterli stok yok.");

        bread.StockQuantity -= quantity; // Raftan düş
        breadRepository.Update(bread);

        return (bread.Name, bread.Price);
    }

    /// <summary>
    /// Pasta siparişi: satışa açık mı kontrol et (pastada stok sayısı yok).
    /// </summary>
    private async Task<(string Name, decimal Price)> ResolveCakeAsync(
        int productId,
        CancellationToken cancellationToken)
    {
        var cake = await cakeRepository.GetByIdAsync(productId, cancellationToken)
            ?? throw new InvalidOperationException($"Pasta bulunamadı (Id: {productId}).");

        if (!cake.IsAvailable)
            throw new InvalidOperationException($"'{cake.Name}' şu an satışa kapalı.");

        return (cake.Name, cake.Price);
    }

    private static OrderDto MapToDto(Order order)
    {
        var items = order.Items
            .Select(i => new OrderItemDto(i.Id, i.ProductType, i.ProductId, i.ProductName, i.Quantity, i.UnitPrice))
            .ToList();

        return new OrderDto(
            order.Id,
            order.CustomerId,
            order.Customer.FullName,
            order.OrderDate,
            order.Status,
            order.TotalAmount,
            items,
            order.CreatedAt);
    }
}
