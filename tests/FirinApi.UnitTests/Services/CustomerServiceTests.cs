using FirinApi.Models.DTOs.Customers;
using FirinApi.Models.Entities;
using FirinApi.Repositories.Interfaces;
using FirinApi.Services.Implementations;
using Moq;

namespace FirinApi.UnitTests.Services;

public class CustomerServiceTests
{
    private readonly Mock<ICustomerRepository> _repo = new();
    private readonly CustomerService _service;

    public CustomerServiceTests()
    {
        _service = new CustomerService(_repo.Object);
    }

    [Fact]
    public async Task CreateAsync_AyniEmail_HataFirlatir()
    {
        _repo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Customer, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Customer { Email = "ali@test.com" });

        var dto = new CreateCustomerDto("Ali", "ali@test.com", "0555");

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(dto));
        Assert.Contains("zaten kayıtlı", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_GecerliMusteri_Olusturulur()
    {
        _repo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Customer, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);
        _repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var dto = new CreateCustomerDto("Ayşe Yılmaz", "ayse@test.com", "05551234567");

        var result = await _service.CreateAsync(dto);

        Assert.Equal("Ayşe Yılmaz", result.FullName);
        Assert.Equal("ayse@test.com", result.Email);
        _repo.Verify(r => r.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
