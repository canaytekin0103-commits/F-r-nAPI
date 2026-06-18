using FirinApi.Models.Common;
using FirinApi.Models.DTOs.Breads;
using FirinApi.Models.DTOs.Cakes;
using FirinApi.Models.DTOs.Customers;
using FirinApi.Models.DTOs.Orders;

namespace FirinApi.Services.Interfaces;

public interface IBreadService
{
    Task<PagedResult<BreadDto>> GetPagedAsync(PaginationQuery query, CancellationToken cancellationToken = default);
    Task<BreadDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<BreadDto> CreateAsync(CreateBreadDto dto, CancellationToken cancellationToken = default);
    Task<BreadDto?> UpdateAsync(int id, UpdateBreadDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

public interface ICakeService
{
    Task<PagedResult<CakeDto>> GetPagedAsync(PaginationQuery query, CancellationToken cancellationToken = default);
    Task<CakeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CakeDto> CreateAsync(CreateCakeDto dto, CancellationToken cancellationToken = default);
    Task<CakeDto?> UpdateAsync(int id, UpdateCakeDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

public interface ICustomerService
{
    Task<PagedResult<CustomerDto>> GetPagedAsync(PaginationQuery query, CancellationToken cancellationToken = default);
    Task<CustomerDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CustomerDto> CreateAsync(CreateCustomerDto dto, CancellationToken cancellationToken = default);
    Task<CustomerDto?> UpdateAsync(int id, UpdateCustomerDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

public interface IOrderService
{
    Task<PagedResult<OrderDto>> GetPagedAsync(PaginationQuery query, CancellationToken cancellationToken = default);
    Task<OrderDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<OrderDto> CreateAsync(CreateOrderDto dto, CancellationToken cancellationToken = default);
    Task<OrderDto?> UpdateStatusAsync(int id, UpdateOrderStatusDto dto, CancellationToken cancellationToken = default);
    Task<OrderDto?> CancelAsync(int id, CancellationToken cancellationToken = default);
}
