using LibrarySystem.Application.Interfaces;
using MediatR;
using LibrarySystem.Application.DTOs.ItemCopies;

namespace LibrarySystem.Application.Commands.ItemCopies;


public class UndeleteItemCopyCommandHandler : IRequestHandler<UndeleteItemCopyCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UndeleteItemCopyCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(UndeleteItemCopyCommand request, CancellationToken cancellationToken)
    {
        var results = await _unitOfWork.ItemCopies.FindWithDeletedAsync(c => c.Id == request.Id);

        if (results == null) return false;

        var entity = results.FirstOrDefault();
        if (entity == null) return false;

        entity.IsDeleted = false;
        entity.DeletedAt = null;

        _unitOfWork.ItemCopies.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
