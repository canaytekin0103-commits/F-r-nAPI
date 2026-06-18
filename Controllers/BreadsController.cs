using FirinApi.Models.Common;
using FirinApi.Models.DTOs.Breads;
using FirinApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FirinApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class BreadsController(IBreadService breadService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<BreadDto>>> GetAll(
        [FromQuery] PaginationQuery query,
        CancellationToken cancellationToken)
        => Ok(await breadService.GetPagedAsync(query, cancellationToken));

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<BreadDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var bread = await breadService.GetByIdAsync(id, cancellationToken);
        return bread is null ? NotFound() : Ok(bread);
    }

    [HttpPost]
    public async Task<ActionResult<BreadDto>> Create(CreateBreadDto dto, CancellationToken cancellationToken)
    {
        var bread = await breadService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = bread.Id }, bread);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<BreadDto>> Update(int id, UpdateBreadDto dto, CancellationToken cancellationToken)
    {
        var bread = await breadService.UpdateAsync(id, dto, cancellationToken);
        return bread is null ? NotFound() : Ok(bread);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await breadService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
