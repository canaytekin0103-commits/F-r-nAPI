using FirinApi.Models.Common;
using FirinApi.Models.DTOs.Customers;
using FirinApi.Models.Entities;
using FirinApi.Repositories.Interfaces;
using FirinApi.Services.Interfaces;

namespace FirinApi.Services.Implementations;

public class CustomerService(ICustomerRepository repository) : ICustomerService
{
    public async Task<PagedResult<CustomerDto>> GetPagedAsync(PaginationQuery query, CancellationToken cancellationToken = default)
    {
        query.Normalize();
        var (items, totalCount) = await repository.GetPagedAsync(query.Skip, query.Size, cancellationToken);
        return new PagedResult<CustomerDto>
        {
            Items = items.Select(MapToDto).ToList(),
            Page = query.Page,
            PageSize = query.Size,
            TotalCount = totalCount
        };
    }

    public async Task<CustomerDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var customer = await repository.GetByIdAsync(id, cancellationToken);
        return customer is null ? null : MapToDto(customer);
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto, CancellationToken cancellationToken = default)
    {
        var existing = await repository.FindAsync(c => c.Email == dto.Email, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException("Bu e-posta adresi zaten kayıtlı.");

        var customer = new Customer
        {
            FullName = dto.FullName,
            Email = dto.Email,
            Phone = dto.Phone
        };

        await repository.AddAsync(customer, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return MapToDto(customer);
    }

    public async Task<CustomerDto?> UpdateAsync(int id, UpdateCustomerDto dto, CancellationToken cancellationToken = default)
    {
        var customer = await repository.GetByIdAsync(id, cancellationToken);
        if (customer is null) return null;

        var emailTaken = await repository.FindAsync(
            c => c.Email == dto.Email && c.Id != id,
            cancellationToken);

        if (emailTaken is not null)
            throw new InvalidOperationException("Bu e-posta adresi başka bir müşteriye ait.");

        customer.FullName = dto.FullName;
        customer.Email = dto.Email;
        customer.Phone = dto.Phone;

        repository.Update(customer);
        await repository.SaveChangesAsync(cancellationToken);
        return MapToDto(customer);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var customer = await repository.GetByIdAsync(id, cancellationToken);
        if (customer is null) return false;

        repository.Remove(customer);
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static CustomerDto MapToDto(Customer customer)
        => new(customer.Id, customer.FullName, customer.Email, customer.Phone, customer.CreatedAt);
}
