using FirinApi.Models.Enums;

namespace FirinApi.Models.DTOs.Orders;

public record OrderItemDto(
    int Id,
    ProductType ProductType,
    int ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice);

public record OrderDto(
    int Id,
    int CustomerId,
    string CustomerName,
    DateTime OrderDate,
    OrderStatus Status,
    decimal TotalAmount,
    IReadOnlyList<OrderItemDto> Items,
    DateTime CreatedAt);

public record CreateOrderItemDto(ProductType ProductType, int ProductId, int Quantity);

public record CreateOrderDto(int CustomerId, IReadOnlyList<CreateOrderItemDto> Items);

public record UpdateOrderStatusDto(OrderStatus Status);
