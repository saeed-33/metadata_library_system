using LibrarySystem.Application.Commands.ItemCopies;
using LibrarySystem.Application.Queries.ItemCopies;
using LibrarySystem.Domain.common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.API.Controllers;

[Authorize]
[ApiController]
[Route("api/item-copies")]
public class ItemCopiesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ItemCopiesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetByItem([FromQuery] int itemId)
    {
        var copies = await _mediator.Send(new GetItemCopiesQuery(itemId));
        return Ok(copies);
    }

    [HttpGet("WithDeleted")]
    [Authorize(Roles = SystemRoles.Admin)]
    public async Task<IActionResult> GetAllWithDeleted()
    {
        var copies = await _mediator.Send(new GetAllItemCopiesWithDeletedQuery());
        return Ok(copies);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var copy = await _mediator.Send(new GetItemCopyByIdQuery(id));
        return copy == null ? NotFound() : Ok(copy);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateItemCopyCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateItemCopyCommand command)
    {
        if (id != command.Id)
            return BadRequest("URL id does not match command id.");

        var updated = await _mediator.Send(command);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _mediator.Send(new DeleteItemCopyCommand(id));
        return deleted ? NoContent() : NotFound();
    }

    [HttpPut("Undelete/{id:int}")]
    [Authorize(Roles = SystemRoles.Admin)]
    public async Task<IActionResult> Undelete(int id, UndeleteItemCopyCommand command)
    {
        if (id != command.Id)
            return BadRequest("URL id does not match command id.");

        var updated = await _mediator.Send(command);
        return updated ? NoContent() : NotFound();
    }
}
