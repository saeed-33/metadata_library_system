using LibrarySystem.Application.Commands.Bookmarks;
using LibrarySystem.Application.Commands.Features;
using LibrarySystem.Application.Queries.Bookmarks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibrarySystem.API.Controllers;

[Authorize] // إجباري أن يكون المستخدم مسجل الدخول (يمتلك Token)
[ApiController]
[Route("api/[controller]")]
public class PatronsController : ControllerBase
{
    private readonly IMediator _mediator;
    public PatronsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePatronCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(id);
    }
    
    // يمكنك إضافة GET لجلب قائمة المستعيرين هنا لاحقاً
}
