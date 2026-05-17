using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Commands.Users;

public record DeleteSystemUserCommand(int Id) : IRequest<bool>;

public class DeleteSystemUserCommandHandler : IRequestHandler<DeleteSystemUserCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public DeleteSystemUserCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(DeleteSystemUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.SystemUsers.GetByIdAsync(request.Id);
        if (user == null) return false;

        _unitOfWork.SystemUsers.Delete(user);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}