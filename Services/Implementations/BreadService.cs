using FirinApi.Models.Common;
using FirinApi.Models.DTOs.Breads;
using FirinApi.Models.Entities;
using FirinApi.Repositories.Interfaces;
using FirinApi.Services.Interfaces;

namespace FirinApi.Services.Implementations;

public class BreadService(IBreadRepository repository) : IBreadService
{
    public async Task<PagedResult<BreadDto>> GetPagedAsync(PaginationQuery query, CancellationToken cancellationToken = default)
    {
        query.Normalize();
        var (items, totalCount) = await repository.GetPagedAsync(query.Skip, query.Size, cancellationToken);
        return new PagedResult<BreadDto>
        {
            Items = items.Select(MapToDto).ToList(),
            Page = query.Page,
            PageSize = query.Size,
            TotalCount = totalCount
        };
    }

    public async Task<BreadDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var bread = await repository.GetByIdAsync(id, cancellationToken);
        return bread is null ? null : MapToDto(bread);
    }

    public async Task<BreadDto> CreateAsync(CreateBreadDto dto, CancellationToken cancellationToken = default)
    {
        var bread = new Bread
        {
            Name = dto.Name,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity
        };

        await repository.AddAsync(bread, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return MapToDto(bread);
    }

    public async Task<BreadDto?> UpdateAsync(int id, UpdateBreadDto dto, CancellationToken cancellationToken = default)
    {
        var bread = await repository.GetByIdAsync(id, cancellationToken);
        if (bread is null) return null;

        bread.Name = dto.Name;
        bread.Price = dto.Price;
        bread.StockQuantity = dto.StockQuantity;

        repository.Update(bread);
        await repository.SaveChangesAsync(cancellationToken);
        return MapToDto(bread);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var bread = await repository.GetByIdAsync(id, cancellationToken);
        if (bread is null) return false;

        repository.Remove(bread);
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static BreadDto MapToDto(Bread bread)
        => new(bread.Id, bread.Name, bread.Price, bread.StockQuantity, bread.CreatedAt);
}
