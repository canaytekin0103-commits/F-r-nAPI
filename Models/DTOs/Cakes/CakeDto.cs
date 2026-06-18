namespace FirinApi.Models.DTOs.Cakes;

public record CakeDto(int Id, string Name, string? Description, decimal Price, bool IsAvailable, DateTime CreatedAt);

public record CreateCakeDto(string Name, string? Description, decimal Price, bool IsAvailable = true);

public record UpdateCakeDto(string Name, string? Description, decimal Price, bool IsAvailable);
