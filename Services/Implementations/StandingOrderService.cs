using FirinApi.Models.DTOs.StandingOrders;
using FirinApi.Models.Entities;
using FirinApi.Repositories.Interfaces;
using FirinApi.Services.Interfaces;

namespace FirinApi.Services.Implementations;

public class StandingOrderService(
    IStandingOrderRepository standingOrderRepository,
    ICustomerRepository customerRepository,
    IBreadRepository breadRepository) : IStandingOrderService
{
    public async Task<IReadOnlyList<StandingOrderDto>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default)
    {
        var items = await standingOrderRepository.GetByCustomerIdAsync(customerId, cancellationToken);
        return items.Select(MapToDto).ToList();
    }

    public async Task<StandingOrderDto> CreateAsync(int customerId, CreateStandingOrderDto dto, CancellationToken cancellationToken = default)
    {
        _ = await customerRepository.GetByIdAsync(customerId, cancellationToken)
            ?? throw new InvalidOperationException("Müşteri bulunamadı.");

        var bread = await breadRepository.GetByIdAsync(dto.BreadId, cancellationToken)
            ?? throw new InvalidOperationException("Ürün bulunamadı.");

        if (dto.Quantity <= 0)
            throw new InvalidOperationException("Adet 0'dan büyük olmalıdır.");

        var existing = await standingOrderRepository.GetByCustomerIdAsync(customerId, cancellationToken);
        if (existing.Any(s => s.BreadId == dto.BreadId))
            throw new InvalidOperationException($"'{bread.Name}' bu müşteri için zaten tanımlı.");

        var item = new CustomerStandingOrder
        {
            CustomerId = customerId,
            BreadId = dto.BreadId,
            Quantity = dto.Quantity
        };

        await standingOrderRepository.AddAsync(item, cancellationToken);
        await standingOrderRepository.SaveChangesAsync(cancellationToken);

        item.Bread = bread;
        return MapToDto(item);
    }

    public async Task<StandingOrderDto?> UpdateAsync(int customerId, int id, UpdateStandingOrderDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.Quantity <= 0)
            throw new InvalidOperationException("Adet 0'dan büyük olmalıdır.");

        var item = await standingOrderRepository.GetByIdAsync(id, cancellationToken);
        if (item is null || item.CustomerId != customerId)
            return null;

        item.Quantity = dto.Quantity;
        standingOrderRepository.Update(item);
        await standingOrderRepository.SaveChangesAsync(cancellationToken);

        var withBread = (await standingOrderRepository.GetByCustomerIdAsync(customerId, cancellationToken))
            .First(s => s.Id == id);
        return MapToDto(withBread);
    }

    public async Task<bool> DeleteAsync(int customerId, int id, CancellationToken cancellationToken = default)
    {
        var item = await standingOrderRepository.GetByIdAsync(id, cancellationToken);
        if (item is null || item.CustomerId != customerId)
            return false;

        standingOrderRepository.Remove(item);
        await standingOrderRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static StandingOrderDto MapToDto(CustomerStandingOrder item)
        => new(item.Id, item.CustomerId, item.BreadId, item.Bread.Name, item.Quantity);
}
