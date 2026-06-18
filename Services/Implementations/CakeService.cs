using FirinApi.Models.Common;
using FirinApi.Models.DTOs.Cakes;
using FirinApi.Models.Entities;
using FirinApi.Repositories.Interfaces;
using FirinApi.Services.Interfaces;

namespace FirinApi.Services.Implementations;

public class CakeService(ICakeRepository repository) : ICakeService
{
    public async Task<PagedResult<CakeDto>> GetPagedAsync(PaginationQuery query, CancellationToken cancellationToken = default)
    {
        query.Normalize();
        var (items, totalCount) = await repository.GetPagedAsync(query.Skip, query.Size, cancellationToken);
        return new PagedResult<CakeDto>
        {
            Items = items.Select(MapToDto).ToList(),
            Page = query.Page,
            PageSize = query.Size,
            TotalCount = totalCount
        };
    }

    public async Task<CakeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var cake = await repository.GetByIdAsync(id, cancellationToken);
        return cake is null ? null : MapToDto(cake);
    }

    public async Task<CakeDto> CreateAsync(CreateCakeDto dto, CancellationToken cancellationToken = default)
    {
        var cake = new Cake
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            IsAvailable = dto.IsAvailable
        };

        await repository.AddAsync(cake, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return MapToDto(cake);
    }

    public async Task<CakeDto?> UpdateAsync(int id, UpdateCakeDto dto, CancellationToken cancellationToken = default)
    {
        var cake = await repository.GetByIdAsync(id, cancellationToken);
        if (cake is null) return null;

        cake.Name = dto.Name;
        cake.Description = dto.Description;
        cake.Price = dto.Price;
        cake.IsAvailable = dto.IsAvailable;

        repository.Update(cake);
        await repository.SaveChangesAsync(cancellationToken);
        return MapToDto(cake);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var cake = await repository.GetByIdAsync(id, cancellationToken);
        if (cake is null) return false;

        repository.Remove(cake);
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static CakeDto MapToDto(Cake cake)
        => new(cake.Id, cake.Name, cake.Description, cake.Price, cake.IsAvailable, cake.CreatedAt);
}
