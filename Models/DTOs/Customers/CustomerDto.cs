namespace FirinApi.Models.DTOs.Customers;

public record CustomerDto(int Id, string FullName, string Email, string? Phone, DateTime CreatedAt);

public record CreateCustomerDto(string FullName, string Email, string? Phone);

public record UpdateCustomerDto(string FullName, string Email, string? Phone);
