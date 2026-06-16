using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Commands.ItemSets;

public record UndeleteItemSetCommand(int Id) : IRequest<bool>;

public class UndeleteItemSetCommandHandler : IRequestHandler<UndeleteItemSetCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UndeleteItemSetCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(UndeleteItemSetCommand request, CancellationToken cancellationToken)
    {
        var restored = await _unitOfWork.ItemSets.RestoreAsync(request.Id);
        if (!restored) return false;

        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}