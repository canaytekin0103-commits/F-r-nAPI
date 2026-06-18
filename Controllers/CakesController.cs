using FirinApi.Models.Common;
using FirinApi.Models.DTOs.Cakes;
using FirinApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FirinApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class CakesController(ICakeService cakeService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<CakeDto>>> GetAll(
        [FromQuery] PaginationQuery query,
        CancellationToken cancellationToken)
        => Ok(await cakeService.GetPagedAsync(query, cancellationToken));

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<CakeDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var cake = await cakeService.GetByIdAsync(id, cancellationToken);
        return cake is null ? NotFound() : Ok(cake);
    }

    [HttpPost]
    public async Task<ActionResult<CakeDto>> Create(CreateCakeDto dto, CancellationToken cancellationToken)
    {
        var cake = await cakeService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = cake.Id }, cake);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CakeDto>> Update(int id, UpdateCakeDto dto, CancellationToken cancellationToken)
    {
        var cake = await cakeService.UpdateAsync(id, dto, cancellationToken);
        return cake is null ? NotFound() : Ok(cake);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await cakeService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
