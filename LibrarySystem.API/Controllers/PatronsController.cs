using LibrarySystem.Application.Commands.Patrons;
using LibrarySystem.Application.Queries.Patrons;
using LibrarySystem.Domain.common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PatronsController : ControllerBase
{
    private readonly IMediator _mediator;
    public PatronsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search)
    {
        var result = await _mediator.Send(new GetPatronsQuery(search));
        return Ok(result);
    }

    [HttpGet("AllWithDeleted")]
    [Authorize(Roles = SystemRoles.Admin)]
    public async Task<IActionResult> GetAllWithDeleted()
    {
        var result = await _mediator.Send(new GetAllPatronsWithDeletedQuery());
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var patron = await _mediator.Send(new GetPatronByIdQuery(id));
        return patron == null ? NotFound() : Ok(patron);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePatronCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePatronCommand command)
    {
        if (id != command.Id)
            return BadRequest("URL id does not match command id.");

        var updated = await _mediator.Send(command);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _mediator.Send(new DeletePatronCommand(id));
        return deleted ? NoContent() : NotFound();
    }

    [HttpPut("Undelete/{id:int}")]
    [Authorize(Roles = SystemRoles.Admin)]
    public async Task<IActionResult> Undelete(int id, UndeletePatronCommand command)
    {
        if (id != command.Id)
            return BadRequest("URL id does not match command id.");

        var updated = await _mediator.Send(command);
        return updated ? NoContent() : NotFound();
    }
}
