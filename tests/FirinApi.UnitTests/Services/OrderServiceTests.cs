using FirinApi.Models.DTOs.Orders;
using FirinApi.Models.Entities;
using FirinApi.Models.Enums;
using FirinApi.Repositories.Interfaces;
using FirinApi.Services.Implementations;
using Moq;

namespace FirinApi.UnitTests.Services;

/// <summary>
/// Sipariş servisi testleri — stok düşme, iptal ve iade kuralları.
/// </summary>
public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepo = new();
    private readonly Mock<ICustomerRepository> _customerRepo = new();
    private readonly Mock<IBreadRepository> _breadRepo = new();
    private readonly Mock<ICakeRepository> _cakeRepo = new();
    private readonly OrderService _service;

    public OrderServiceTests()
    {
        _service = new OrderService(
            _orderRepo.Object,
            _customerRepo.Object,
            _breadRepo.Object,
            _cakeRepo.Object);
    }

    [Fact]
    public async Task CreateAsync_EkmekSiparisi_StokDuser()
    {
        // Arrange — hazırlık: 50 stoklu ekmek, 5 adet sipariş
        var customer = new Customer { Id = 1, FullName = "Ali Test" };
        var bread = new Bread { Id = 1, Name = "Tam Buğday", Price = 35m, StockQuantity = 50 };

        _customerRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(customer);
        _breadRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(bread);
        _orderRepo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var dto = new CreateOrderDto(1, [new CreateOrderItemDto(ProductType.Bread, 1, 5)]);

        // Act — çalıştır
        var result = await _service.CreateAsync(dto);

        // Assert — doğrula: stok 45, tutar 175
        Assert.Equal(45, bread.StockQuantity);
        Assert.Equal(175m, result.TotalAmount);
        _breadRepo.Verify(r => r.Update(bread), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_YetersizStok_HataFirlatir()
    {
        var customer = new Customer { Id = 1, FullName = "Ali" };
        var bread = new Bread { Id = 1, Name = "Tam Buğday", Price = 35m, StockQuantity = 3 };

        _customerRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(customer);
        _breadRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(bread);

        var dto = new CreateOrderDto(1, [new CreateOrderItemDto(ProductType.Bread, 1, 5)]);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(dto));
        Assert.Contains("yeterli stok yok", ex.Message);
    }

    [Fact]
    public async Task CancelAsync_EkmekSiparisi_StokGeriIadeEdilir()
    {
        var customer = new Customer { Id = 1, FullName = "Ali Test" };
        var bread = new Bread { Id = 1, Name = "Tam Buğday", Price = 35m, StockQuantity = 45 };
        var order = new Order
        {
            Id = 1,
            CustomerId = 1,
            Customer = customer,
            Status = OrderStatus.Pending,
            TotalAmount = 175m,
            Items =
            [
                new OrderItem
                {
                    ProductType = ProductType.Bread,
                    ProductId = 1,
                    ProductName = "Tam Buğday",
                    Quantity = 5,
                    UnitPrice = 35m
                }
            ]
        };

        _orderRepo.Setup(r => r.GetByIdWithDetailsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(order);
        _breadRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(bread);
        _orderRepo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _service.CancelAsync(1);

        Assert.NotNull(result);
        Assert.Equal(OrderStatus.Cancelled, result.Status);
        Assert.Equal(50, bread.StockQuantity);
    }

    [Fact]
    public async Task CancelAsync_TamamlanmisSiparis_HataFirlatir()
    {
        var order = new Order
        {
            Id = 1,
            Customer = new Customer { FullName = "Ali" },
            Status = OrderStatus.Completed,
            Items = []
        };

        _orderRepo.Setup(r => r.GetByIdWithDetailsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(order);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CancelAsync(1));
        Assert.Contains("Tamamlanmış sipariş", ex.Message);
    }

    [Fact]
    public async Task CancelAsync_ZatenIptalEdilmis_HataFirlatir()
    {
        var order = new Order
        {
            Id = 1,
            Customer = new Customer { FullName = "Ali" },
            Status = OrderStatus.Cancelled,
            Items = []
        };

        _orderRepo.Setup(r => r.GetByIdWithDetailsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(order);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CancelAsync(1));
        Assert.Contains("zaten iptal", ex.Message);
    }
}
