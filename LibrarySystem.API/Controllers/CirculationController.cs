using LibrarySystem.Application.Commands.Bookmarks;
using LibrarySystem.Application.Commands.Circulation;
using LibrarySystem.Application.Queries.Bookmarks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibrarySystem.API.Controllers;


[Authorize] // إجباري أن يكون المستخدم مسجل الدخول (يمتلك Token)
[ApiController]
[Route("api/[controller]")]
public class CirculationController : ControllerBase
{
    private readonly IMediator _mediator;
    public CirculationController(IMediator mediator) => _mediator = mediator;

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new { success = result, message = "تمت عملية الإعارة بنجاح" });
    }

    [HttpPost("return")]
    public async Task<IActionResult> Return([FromBody] ReturnCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new { success = result, message = "تم إرجاع النسخة بنجاح" });
    }
}
