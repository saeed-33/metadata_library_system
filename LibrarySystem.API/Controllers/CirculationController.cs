using LibrarySystem.Application.Commands.Circulation;
using LibrarySystem.Application.Queries.Circulation;
using LibrarySystem.Application.Queries.Patrons;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LibrarySystem.Application.DTOs.Circulation;

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

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveLoans()
    {
        var result = await _mediator.Send(new GetActiveLoansQuery());
        return Ok(result);
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory([FromQuery] int? itemId, [FromQuery] int? patronId)
    {
        var result = await _mediator.Send(new GetBorrowHistoryQuery(itemId, patronId));
        return Ok(result);
    }

    [HttpGet("overdue")]
    public async Task<IActionResult> GetOverdue()
    {
        var result = await _mediator.Send(new GetOverdueLoansQuery());
        return Ok(result);
    }
}
