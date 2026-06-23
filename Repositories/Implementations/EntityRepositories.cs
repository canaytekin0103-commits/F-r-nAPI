using FirinApi.Data;
using FirinApi.Models.Entities;
using FirinApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FirinApi.Repositories.Implementations;

public class BreadRepository(AppDbContext context) : Repository<Bread>(context), IBreadRepository;

public class CakeRepository(AppDbContext context) : Repository<Cake>(context), ICakeRepository;

public class CustomerRepository(AppDbContext context) : Repository<Customer>(context), ICustomerRepository;

public class OrderRepository(AppDbContext context) : Repository<Order>(context), IOrderRepository
{
    public async Task<IReadOnlyList<Order>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
        => await Context.Orders
            .AsNoTracking()
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync(cancellationToken);

    public async Task<(IReadOnlyList<Order> Items, int TotalCount)> GetPagedWithDetailsAsync(
        int skip, int take, CancellationToken cancellationToken = default)
    {
        var query = Context.Orders
            .AsNoTracking()
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .OrderByDescending(o => o.OrderDate);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Skip(skip).Take(take).ToListAsync(cancellationToken);
        return (items, totalCount);
    }

    public async Task<Order?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        => await Context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Order>> GetDeliveriesByDateAsync(DateOnly date, CancellationToken cancellationToken = default)
        => await Context.Orders
            .AsNoTracking()
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .Where(o => o.DeliveryDate == date && o.Status == Models.Enums.OrderStatus.Completed)
            .OrderBy(o => o.Customer.FullName)
            .ToListAsync(cancellationToken);

    public async Task<bool> HasDeliveryForCustomerOnDateAsync(int customerId, DateOnly date, CancellationToken cancellationToken = default)
        => await Context.Orders.AnyAsync(
            o => o.CustomerId == customerId
                 && o.DeliveryDate == date
                 && o.Status == Models.Enums.OrderStatus.Completed,
            cancellationToken);
}

public class StandingOrderRepository(AppDbContext context) : Repository<CustomerStandingOrder>(context), IStandingOrderRepository
{
    public async Task<IReadOnlyList<CustomerStandingOrder>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default)
        => await Context.CustomerStandingOrders
            .AsNoTracking()
            .Include(s => s.Bread)
            .Where(s => s.CustomerId == customerId)
            .OrderBy(s => s.Bread.Name)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Customer>> GetCustomersWithStandingOrdersAsync(CancellationToken cancellationToken = default)
        => await Context.Customers
            .AsNoTracking()
            .Include(c => c.StandingOrders)
                .ThenInclude(s => s.Bread)
            .Where(c => c.StandingOrders.Any())
            .OrderBy(c => c.FullName)
            .ToListAsync(cancellationToken);
}
