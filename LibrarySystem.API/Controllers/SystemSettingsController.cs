using LibrarySystem.Application.Commands.SystemSettings;
using LibrarySystem.Application.Queries.SystemSettings;
using LibrarySystem.Domain.common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.API.Controllers;

[ApiController]
[Route("api/system-settings")]


public class SystemSettingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SystemSettingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [AllowAnonymous]

    [Authorize(Roles = $"{SystemRoles.Admin},{SystemRoles.Librarian}")]

    public async Task<IActionResult> GetAll()
    {
        var settings = await _mediator.Send(new GetSystemSettingsQuery());
        return Ok(settings);
    }

    [HttpPut("{key}")]
    [Authorize(Roles = SystemRoles.Admin)]
    public async Task<IActionResult> Update(string key, [FromBody] string value)
    {
        var updated = await _mediator.Send(new UpdateSystemSettingCommand(key, value));
        return updated ? NoContent() : NotFound();
    }
}
