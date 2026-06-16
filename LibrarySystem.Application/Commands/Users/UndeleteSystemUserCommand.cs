using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Commands.Users;

public record UndeleteSystemUserCommand(int Id) : IRequest<bool>;

public class UndeleteSystemUserCommandHandler : IRequestHandler<UndeleteSystemUserCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UndeleteSystemUserCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(UndeleteSystemUserCommand request, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.SystemUsers.FindWithDeletedAsync(u => u.Id == request.Id);
        var user = users.FirstOrDefault();

        if (user == null) return false;

        user.IsDeleted = false;
        user.DeletedAt = null;

        _unitOfWork.SystemUsers.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}