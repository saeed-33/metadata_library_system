using LibrarySystem.Application.Commands.ResourceTemplates;
using LibrarySystem.Application.Commands.Users;
using LibrarySystem.Application.Queries.Users;
using LibrarySystem.Domain.common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = SystemRoles.Admin)]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Only Admin can see all users
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _mediator.Send(new GetAllUsersQuery());
        return Ok(users);
    }
    
    [HttpGet("withDeleted")]
    public async Task<IActionResult> GetAllWitDeleted()
    {
        var users = await _mediator.Send(new GetAllWithDeletedUsersQuery());
        return Ok(users);
    }
    // Admin can get any user, others can only get themselves
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _mediator.Send(new GetSystemUserByIdQuery(id));
        return user == null ? NotFound() : Ok(user);
    }

    // Only Admin can create users manually
    // Normal users are created automatically during Register in AuthController
    [HttpPost]
    public async Task<IActionResult> Create(CreateSystemUserCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateSystemUserCommand command)
    {
        if (id != command.Id)
            return BadRequest("URL id does not match command id.");

        var updated = await _mediator.Send(command);
        return updated ? NoContent() : NotFound();
    }

    // Only Admin can delete users
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _mediator.Send(new DeleteSystemUserCommand(id));
        return deleted ? NoContent() : NotFound();
    }

    [HttpPut("Undelete/{id:int}")]
    public async Task<IActionResult> Undelete(int id, UndeleteSystemUserCommand command)
    {
        if (id != command.Id)
            return BadRequest("URL id does not match command id.");

        var updated = await _mediator.Send(command);
        return updated ? NoContent() : NotFound();
    }
    //NEW ENDPOINT: Only Admin can update a user's roles
    [HttpPut("{id:int}/roles")]
    public async Task<IActionResult> UpdateRoles(int id, UpdateSystemUserRolesCommand command)
    {
        if (id != command.Id)
            return BadRequest("URL id does not match command id.");

        var updated = await _mediator.Send(command);
        return updated ? NoContent() : NotFound();
    }

}