using FirinApi.Models.DTOs.StandingOrders;
using FirinApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FirinApi.Controllers;

[ApiController]
[Route("api/customers/{customerId:int}/standing-orders")]
[Authorize(Roles = "Admin")]
public class StandingOrdersController(IStandingOrderService standingOrderService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StandingOrderDto>>> GetAll(
        int customerId,
        CancellationToken cancellationToken)
        => Ok(await standingOrderService.GetByCustomerIdAsync(customerId, cancellationToken));

    [HttpPost]
    public async Task<ActionResult<StandingOrderDto>> Create(
        int customerId,
        CreateStandingOrderDto dto,
        CancellationToken cancellationToken)
    {
        var item = await standingOrderService.CreateAsync(customerId, dto, cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { customerId }, item);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<StandingOrderDto>> Update(
        int customerId,
        int id,
        UpdateStandingOrderDto dto,
        CancellationToken cancellationToken)
    {
        var item = await standingOrderService.UpdateAsync(customerId, id, dto, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int customerId, int id, CancellationToken cancellationToken)
    {
        var deleted = await standingOrderService.DeleteAsync(customerId, id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
