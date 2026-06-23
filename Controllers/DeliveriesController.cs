using FirinApi.Models.DTOs.Deliveries;
using FirinApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FirinApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class DeliveriesController(IDeliveryService deliveryService) : ControllerBase
{
    /// <summary>
    /// Seçilen güne ait teslimat özeti (kim ne aldı, toplam ürünler).
    /// </summary>
    [HttpGet("daily")]
    public async Task<ActionResult<DailyDeliverySummaryDto>> GetDaily(
        [FromQuery] DateOnly? date,
        CancellationToken cancellationToken)
    {
        var targetDate = date ?? DateOnly.FromDateTime(DateTime.UtcNow);
        return Ok(await deliveryService.GetDailySummaryAsync(targetDate, cancellationToken));
    }

    /// <summary>
    /// Düzenli müşterilerin o günkü teslimatlarını kaydeder (durum: Verildi).
    /// Aynı gün için zaten kayıtlı müşteriler atlanır.
    /// </summary>
    [HttpPost("record-day")]
    public async Task<ActionResult<RecordDayResultDto>> RecordDay(
        RecordDayDto dto,
        CancellationToken cancellationToken)
        => Ok(await deliveryService.RecordDayAsync(dto, cancellationToken));
}
