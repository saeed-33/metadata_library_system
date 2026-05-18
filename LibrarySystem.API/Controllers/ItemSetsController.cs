using LibrarySystem.Application.Commands.ItemSets;
using LibrarySystem.Application.DTOs.ItemSets;
using LibrarySystem.Application.Queries.ItemSets;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.API.Controllers;

[ApiController]
[Route("api/item-sets")]
public class ItemSetsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ItemSetsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var itemSets = await _mediator.Send(new GetAllItemSetsQuery());
        return Ok(itemSets);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var itemSet = await _mediator.Send(new GetItemSetByIdQuery(id));
        return itemSet == null ? NotFound() : Ok(itemSet);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateItemSetRequest request)
    {
        var id = await _mediator.Send(new CreateItemSetCommand(
            request.Title,
            request.Description,
            request.IsPublic,
            request.OwnerId));

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateItemSetRequest request)
    {
        var updated = await _mediator.Send(new UpdateItemSetCommand(
            id,
            request.Title,
            request.Description,
            request.IsPublic,
            request.OwnerId));

        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _mediator.Send(new DeleteItemSetCommand(id));
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("{itemSetId:int}/items/{itemId:int}")]
    public async Task<IActionResult> AddItem(int itemSetId, int itemId)
    {
        var added = await _mediator.Send(new AddItemToItemSetCommand(itemSetId, itemId));
        return added ? NoContent() : NotFound();
    }

    [HttpDelete("{itemSetId:int}/items/{itemId:int}")]
    public async Task<IActionResult> RemoveItem(int itemSetId, int itemId)
    {
        var removed = await _mediator.Send(new RemoveItemFromItemSetCommand(itemSetId, itemId));
        return removed ? NoContent() : NotFound();
    }
}
