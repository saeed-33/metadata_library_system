using LibrarySystem.Application.Commands;
using LibrarySystem.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.API.Controllers;

[ApiController]
[Route("api/vocabularies")]
public class VocabulariesController : ControllerBase
{
    private readonly IMediator _mediator;

    public VocabulariesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var vocabularies = await _mediator.Send(new GetAllVocabulariesQuery());
        return Ok(vocabularies);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var vocabulary = await _mediator.Send(new GetVocabularyByIdQuery(id));
        return vocabulary == null ? NotFound() : Ok(vocabulary);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateVocabularyCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateVocabularyCommand command)
    {
        // Make sure the id in the URL matches the command
        if (id != command.Id)
            return BadRequest("URL id does not match command id.");

        var updated = await _mediator.Send(command);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _mediator.Send(new DeleteVocabularyCommand(id));
        return deleted ? NoContent() : NotFound();
    }
}