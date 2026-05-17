using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Commands.ItemSets;

public record AddItemToItemSetCommand(int ItemSetId, int ItemId) : IRequest<bool>;

public class AddItemToItemSetCommandHandler : IRequestHandler<AddItemToItemSetCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddItemToItemSetCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(AddItemToItemSetCommand request, CancellationToken cancellationToken)
    {
        var added = await _unitOfWork.ItemSets.AddItemAsync(request.ItemSetId, request.ItemId);
        if (!added) return false;

        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
