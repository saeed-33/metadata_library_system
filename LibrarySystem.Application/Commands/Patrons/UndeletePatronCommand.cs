using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Commands.Patrons;

public record UndeletePatronCommand(int Id) : IRequest<bool>;

public class UndeletePatronCommandHandler : IRequestHandler<UndeletePatronCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UndeletePatronCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(UndeletePatronCommand request, CancellationToken cancellationToken)
    {
        var results = await _unitOfWork.Patrons.FindWithDeletedAsync(p => p.Id == request.Id);

        if (results == null) return false;

        var entity = results.FirstOrDefault();
        if (entity == null) return false;

        entity.IsDeleted = false;
        entity.DeletedAt = null;

        _unitOfWork.Patrons.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
