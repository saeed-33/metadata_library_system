using LibrarySystem.Application.Commands.Items;
using LibrarySystem.Application.Queries.Items;
using LibrarySystem.Domain.common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.API.Controllers;

[Authorize]
[ApiController]
[Route("api/items")]
public class ItemsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ItemsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _mediator.Send(new GetAllItemsQuery());
        return Ok(items);
    }

    [HttpGet("withDeleted")]
    [Authorize(Roles = SystemRoles.Admin)]

    public async Task<IActionResult> GetAllWitDeleted()
    {
        var items = await _mediator.Send(new GetAllItemsWithDeletedQuery());
        return Ok(items);
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _mediator.Send(new GetItemByIdQuery(id));
        return item == null ? NotFound() : Ok(item);
    }

    [HttpGet("{id:int}/copies")]
    public async Task<IActionResult> GetWithCopies(int id)
    {
        var item = await _mediator.Send(new GetItemWithCopiesQuery(id));
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateItemCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateItemCommand command)
    {
        if (id != command.Id)
            return BadRequest("URL id does not match command id.");

        var updated = await _mediator.Send(command);
        return updated ? NoContent() : NotFound();
    }

    [HttpPut("Undelete/{id:int}")]
    [Authorize(Roles = SystemRoles.Admin)]
    public async Task<IActionResult> Undelet(int id, UndeletItemCommand command)
    {
        if (id != command.Id)
            return BadRequest("URL id does not match command id.");

        var updated = await _mediator.Send(command);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _mediator.Send(new DeleteItemCommand(id));
        return deleted ? NoContent() : NotFound();
    }
}