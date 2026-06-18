using FirinApi.Models.Common;
using FirinApi.Models.DTOs.Orders;
using FirinApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FirinApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<OrderDto>>> GetAll(
        [FromQuery] PaginationQuery query,
        CancellationToken cancellationToken)
        => Ok(await orderService.GetPagedAsync(query, cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var order = await orderService.GetByIdAsync(id, cancellationToken);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(CreateOrderDto dto, CancellationToken cancellationToken)
    {
        var order = await orderService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    /// <summary>
    /// Sipariş durumunu günceller.
    /// Status = 3 (Cancelled) gönderilirse stok otomatik iade edilir.
    /// </summary>
    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<OrderDto>> UpdateStatus(
        int id,
        UpdateOrderStatusDto dto,
        CancellationToken cancellationToken)
    {
        var order = await orderService.UpdateStatusAsync(id, dto, cancellationToken);
        return order is null ? NotFound() : Ok(order);
    }

    /// <summary>
    /// Siparişi iptal eder ve ekmek stoklarını rafa geri koyar.
    /// </summary>
    [HttpPost("{id:int}/cancel")]
    public async Task<ActionResult<OrderDto>> Cancel(int id, CancellationToken cancellationToken)
    {
        var order = await orderService.CancelAsync(id, cancellationToken);
        return order is null ? NotFound() : Ok(order);
    }
}
