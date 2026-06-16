using LibrarySystem.Application.Commands.Properties;
using LibrarySystem.Application.Commands.ResourceTemplates;
using LibrarySystem.Application.Queries.ResourceTemplates;
using LibrarySystem.Domain.common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.API.Controllers;

[ApiController]
[Route("api/resource-templates")]
public class ResourceTemplatesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ResourceTemplatesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var templates = await _mediator.Send(new GetAllResourceTemplatesQuery());
        return Ok(templates);
    }

    [HttpGet("WithDeleted")]
    [Authorize(Roles = SystemRoles.Admin)]

    public async Task<IActionResult> GetAllWithDeleted()
    {
        var templates = await _mediator.Send(new GetAllResourceTemplatesWithDeletedQuery());
        return Ok(templates);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var template = await _mediator.Send(new GetResourceTemplateByIdQuery(id));
        return template == null ? NotFound() : Ok(template);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateResourceTemplateCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateResourceTemplateCommand command)
    {
        if (id != command.Id)
            return BadRequest("URL id does not match command id.");

        var updated = await _mediator.Send(command);
        return updated ? NoContent() : NotFound();
    }

    // This endpoint manages which properties belong to the template
    // PUT api/resource-templates/5/properties
    [HttpPut("{id:int}/properties")]
    public async Task<IActionResult> UpdateProperties(int id, UpdateTemplatePropertiesCommand command)
    {
        if (id != command.TemplateId)
            return BadRequest("URL id does not match command TemplateId.");

        var updated = await _mediator.Send(command);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteResourceTemplateCommand(id));

        if (!result.IsSuccess)
        {
            // Check if it failed because the template is in use (has items)
            // vs not found — return different HTTP status codes
            return result.Message.Contains("غير موجود")
                ? NotFound(result.Message)
                : Conflict(result.Message); // 409 Conflict — can't delete, still in use
        }

        return NoContent();
    }

    [HttpPut("Undelet/{id:int}")]
    public async Task<IActionResult> Undelet(int id, UndeleteResourceTemplateCommand command)
    {
        if (id != command.Id)
            return BadRequest("URL id does not match command id.");

        var updated = await _mediator.Send(command);
        return updated ? NoContent() : NotFound();
    }
}