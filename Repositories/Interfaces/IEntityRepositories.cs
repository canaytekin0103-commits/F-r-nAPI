using FirinApi.Models.Entities;

namespace FirinApi.Repositories.Interfaces;

public interface IBreadRepository : IRepository<Bread> { }

public interface ICakeRepository : IRepository<Cake> { }

public interface ICustomerRepository : IRepository<Customer> { }

public interface IOrderRepository : IRepository<Order>
{
    Task<IReadOnlyList<Order>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Order> Items, int TotalCount)> GetPagedWithDetailsAsync(int skip, int take, CancellationToken cancellationToken = default);
    Task<Order?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
}
