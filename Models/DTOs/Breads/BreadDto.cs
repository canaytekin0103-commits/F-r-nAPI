namespace FirinApi.Models.DTOs.Breads;

public record BreadDto(int Id, string Name, decimal Price, int StockQuantity, DateTime CreatedAt);

public record CreateBreadDto(string Name, decimal Price, int StockQuantity);

public record UpdateBreadDto(string Name, decimal Price, int StockQuantity);
