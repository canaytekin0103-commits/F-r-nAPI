using System.Globalization;
using FirinApi.Models.DTOs.Deliveries;
using FirinApi.Models.DTOs.Orders;
using FirinApi.Models.Enums;
using FirinApi.Repositories.Interfaces;
using FirinApi.Services.Interfaces;

namespace FirinApi.Services.Implementations;

public class DeliveryService(
    IOrderRepository orderRepository,
    IStandingOrderRepository standingOrderRepository,
    IOrderService orderService) : IDeliveryService
{
    public async Task<DailyDeliverySummaryDto> GetDailySummaryAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        var deliveries = await orderRepository.GetDeliveriesByDateAsync(date, cancellationToken);
        var orderDtos = deliveries.Select(MapOrder).ToList();

        var totals = deliveries
            .SelectMany(o => o.Items)
            .GroupBy(i => i.ProductName)
            .Select(g => new DailyProductTotalDto(g.Key, g.Sum(i => i.Quantity)))
            .OrderBy(t => t.ProductName)
            .ToList();

        return new DailyDeliverySummaryDto(
            date,
            GetDayLabel(date),
            orderDtos,
            totals,
            orderDtos.Count,
            orderDtos.Sum(o => o.TotalAmount));
    }

    public async Task<RecordDayResultDto> RecordDayAsync(RecordDayDto dto, CancellationToken cancellationToken = default)
    {
        var date = dto.Date ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var customers = await standingOrderRepository.GetCustomersWithStandingOrdersAsync(cancellationToken);

        if (dto.CustomerIds is { Count: > 0 })
            customers = customers.Where(c => dto.CustomerIds.Contains(c.Id)).ToList();

        var created = new List<OrderDto>();
        var skipped = new List<string>();
        var noStanding = new List<string>();

        foreach (var customer in customers)
        {
            if (!customer.StandingOrders.Any())
            {
                noStanding.Add(customer.FullName);
                continue;
            }

            if (await orderRepository.HasDeliveryForCustomerOnDateAsync(customer.Id, date, cancellationToken))
            {
                skipped.Add(customer.FullName);
                continue;
            }

            var items = customer.StandingOrders
                .Select(s => new CreateOrderItemDto(ProductType.Bread, s.BreadId, s.Quantity))
                .ToList();

            var order = await orderService.CreateAsync(
                new CreateOrderDto(customer.Id, items, MarkAsDelivered: true, DeliveryDate: date),
                cancellationToken);

            created.Add(order);
        }

        return new RecordDayResultDto(date, created, skipped, noStanding);
    }

    private static OrderDto MapOrder(Models.Entities.Order order)
    {
        var items = order.Items
            .Select(i => new OrderItemDto(i.Id, i.ProductType, i.ProductId, i.ProductName, i.Quantity, i.UnitPrice))
            .ToList();

        return new OrderDto(
            order.Id,
            order.CustomerId,
            order.Customer.FullName,
            order.OrderDate,
            order.DeliveryDate,
            order.Status,
            order.TotalAmount,
            items,
            order.CreatedAt);
    }

    private static string GetDayLabel(DateOnly date)
    {
        var dt = date.ToDateTime(TimeOnly.MinValue);
        var culture = new CultureInfo("tr-TR");
        return $"{date:dd MMMM yyyy} — {culture.DateTimeFormat.GetDayName(dt.DayOfWeek)}";
    }
}
