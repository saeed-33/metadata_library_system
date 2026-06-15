using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.common;
using MediatR;

namespace LibrarySystem.Application.Commands.Users;

public record UpdateSystemUserRolesCommand(
    int Id,
    List<string> RoleNames
) : IRequest<bool>;

public class UpdateSystemUserRolesCommandHandler
    : IRequestHandler<UpdateSystemUserRolesCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;

    public UpdateSystemUserRolesCommandHandler(
        IUnitOfWork unitOfWork,
        IIdentityService identityService)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
    }

    public async Task<bool> Handle(
        UpdateSystemUserRolesCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Find the SystemUser to get their ExternalId
        var user = await _unitOfWork.SystemUsers.GetByIdAsync(request.Id);
        if (user == null) return false;

        // 2. Validate that all role names exist
        var validRoles = new List<string>
        {
            SystemRoles.Admin,
            SystemRoles.Librarian,
            SystemRoles.User,
            SystemRoles.Guest
        };

        var invalidRoles = request.RoleNames
            .Where(r => !validRoles.Contains(r))
            .ToList();

        if (invalidRoles.Any()) return false;

        // 3. Update roles in Identity using ExternalId
        return await _identityService
            .UpdateUserRolesAsync(user.ExternalId, request.RoleNames);
    }
}