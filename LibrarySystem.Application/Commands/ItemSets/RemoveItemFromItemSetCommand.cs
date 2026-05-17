using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Commands.ItemSets;

public record RemoveItemFromItemSetCommand(int ItemSetId, int ItemId) : IRequest<bool>;

public class RemoveItemFromItemSetCommandHandler : IRequestHandler<RemoveItemFromItemSetCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public RemoveItemFromItemSetCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(RemoveItemFromItemSetCommand request, CancellationToken cancellationToken)
    {
        var removed = await _unitOfWork.ItemSets.RemoveItemAsync(request.ItemSetId, request.ItemId);
        if (!removed) return false;

        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
