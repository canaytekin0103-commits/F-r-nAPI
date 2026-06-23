namespace FirinApi.Models.DTOs.StandingOrders;

public record StandingOrderDto(
    int Id,
    int CustomerId,
    int BreadId,
    string ProductName,
    int Quantity);

public record CreateStandingOrderDto(int BreadId, int Quantity);

public record UpdateStandingOrderDto(int Quantity);
