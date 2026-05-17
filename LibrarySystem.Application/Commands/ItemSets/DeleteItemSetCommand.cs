using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Commands.ItemSets;

public record DeleteItemSetCommand(int Id) : IRequest<bool>;

public class DeleteItemSetCommandHandler : IRequestHandler<DeleteItemSetCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteItemSetCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(DeleteItemSetCommand request, CancellationToken cancellationToken)
    {
        var deleted = await _unitOfWork.ItemSets.DeleteAsync(request.Id);
        if (!deleted) return false;

        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
