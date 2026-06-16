using LibrarySystem.Application.Commands.Properties;
using LibrarySystem.Application.Queries.Properties;
using LibrarySystem.Domain.common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.API.Controllers;

[ApiController]
[Route("api/properties")]
public class PropertiesController : ControllerBase
{
    private readonly IMediator _mediator;

    public PropertiesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET api/properties
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var properties = await _mediator.Send(new GetAllPropertiesQuery());
        return Ok(properties);
    }


    [HttpGet("WithDeleted")]
    [Authorize(Roles = SystemRoles.Admin)]

    public async Task<IActionResult> GetAllWithDeleted()
    {
        var properties = await _mediator.Send(new GetAllPropertiesWithDeletedQuery());
        return Ok(properties);
    }

    // GET api/properties/by-vocabulary/3
    [HttpGet("by-vocabulary/{vocabularyId:int}")]
    public async Task<IActionResult> GetByVocabulary(int vocabularyId)
    {
        var properties = await _mediator.Send(
            new GetPropertiesByVocabularyQuery(vocabularyId));
        return Ok(properties);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var property = await _mediator.Send(new GetPropertyByIdQuery(id));
        return property == null ? NotFound() : Ok(property);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePropertyCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdatePropertyCommand command)
    {
        if (id != command.Id)
            return BadRequest("URL id does not match command id.");

        var updated = await _mediator.Send(command);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _mediator.Send(new DeletePropertyCommand(id));
        return deleted ? NoContent() : NotFound();
    }
}