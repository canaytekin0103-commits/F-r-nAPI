using FirinApi.Models.DTOs.Orders;

namespace FirinApi.Models.DTOs.Deliveries;

public record DailyProductTotalDto(string ProductName, int Quantity);

public record DailyDeliverySummaryDto(
    DateOnly Date,
    string DayLabel,
    IReadOnlyList<OrderDto> Deliveries,
    IReadOnlyList<DailyProductTotalDto> Totals,
    int CustomerCount,
    decimal TotalAmount);

public record RecordDayDto(DateOnly? Date, IReadOnlyList<int>? CustomerIds);

public record RecordDayResultDto(
    DateOnly Date,
    IReadOnlyList<OrderDto> Created,
    IReadOnlyList<string> SkippedCustomers,
    IReadOnlyList<string> NoStandingOrderCustomers);
