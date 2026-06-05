using LibrarySystem.Application.Commands.Media;
using LibrarySystem.Application.Queries.Media;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.API.Controllers;

[ApiController]
[Route("api/media")]
public class MediaController : ControllerBase
{
    private readonly IMediator _mediator;

    public MediaController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("by-item/{itemId:int}")]
    public async Task<IActionResult> GetByItem(int itemId)
    {
        var mediaList = await _mediator.Send(new GetMediaByItemQuery(itemId));
        return Ok(mediaList);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var media = await _mediator.Send(new GetMediaByIdQuery(id));
        return media == null ? NotFound() : Ok(media);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateMediaCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateMediaCommand command)
    {
        if (id != command.Id)
            return BadRequest("URL id does not match command id.");

        var updated = await _mediator.Send(command);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _mediator.Send(new DeleteMediaCommand(id));
        return deleted ? NoContent() : NotFound();
    }
}