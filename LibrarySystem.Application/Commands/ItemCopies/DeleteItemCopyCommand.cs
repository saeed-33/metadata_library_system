using LibrarySystem.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LibrarySystem.Application.Commands.ItemCopies;

public record DeleteItemCopyCommand(int Id) : IRequest<bool>;

public class DeleteItemCopyCommandHandler : IRequestHandler<DeleteItemCopyCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteItemCopyCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteItemCopyCommand request, CancellationToken cancellationToken)
    {
        var copy = await _unitOfWork.ItemCopies.GetByIdAsync(request.Id);
        if (copy == null) return false;

        copy.IsDeleted = true;
        _unitOfWork.ItemCopies.Update(copy);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
