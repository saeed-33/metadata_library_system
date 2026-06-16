using LibrarySystem.Application.Commands.Bookmarks;
using LibrarySystem.Application.Queries.Bookmarks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibrarySystem.API.Controllers;

[ApiController]
[Route("api/bookmarks")]
[Authorize] // إجباري أن يكون المستخدم مسجل الدخول (يمتلك Token)
public class BookmarksController : ControllerBase
{
    private readonly IMediator _mediator;

    public BookmarksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // أداة مساعدة لاستخراج ExternalId من التوكن (JWT)
    private string? GetUserIdFromToken()
    {
        // إذا كنت تستخدم AspNet Identity فإن الافتراضي هو NameIdentifier
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");
    }

    // 1. جلب مفضلة المستخدم الحالي
    // GET /api/bookmarks
    [HttpGet]
    public async Task<IActionResult> GetBookmarks()
    {
        var externalUserId = GetUserIdFromToken();
        if (string.IsNullOrEmpty(externalUserId)) return Unauthorized();

        var bookmarks = await _mediator.Send(new GetUserBookmarksQuery(externalUserId));

        // سيرجع مصفوفة أرقام: [1, 5, 12]
        return Ok(bookmarks);
    }

    // 2. إضافة عنصر للمفضلة
    // POST /api/bookmarks/{itemId}
    [HttpPost("{itemId:int}")]
    public async Task<IActionResult> AddBookmark(int itemId)
    {
        var externalUserId = GetUserIdFromToken();
        if (string.IsNullOrEmpty(externalUserId)) return Unauthorized();

        var success = await _mediator.Send(new AddBookmarkCommand(externalUserId, itemId));
        if (!success) return BadRequest("Could not add bookmark. User not found.");

        return Ok(); // Status 200
    }

    // 3. إزالة عنصر من المفضلة
    // DELETE /api/bookmarks/{itemId}
    [HttpDelete("{itemId:int}")]
    public async Task<IActionResult> RemoveBookmark(int itemId)
    {
        var externalUserId = GetUserIdFromToken();
        if (string.IsNullOrEmpty(externalUserId)) return Unauthorized();

        var success = await _mediator.Send(new RemoveBookmarkCommand(externalUserId, itemId));
        if (!success) return BadRequest("Could not remove bookmark.");

        return NoContent(); // Status 204
    }
}