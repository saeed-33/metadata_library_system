using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Commands.Properties;

public record UndeletePropertyCommand(int Id) : IRequest<bool>;

public class UndeletePropertyCommandHandler : IRequestHandler<UndeletePropertyCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UndeletePropertyCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(UndeletePropertyCommand request, CancellationToken cancellationToken)
    {
        var properties = await _unitOfWork.Properties.FindWithDeletedAsync(p => p.Id == request.Id);
        var property = properties.FirstOrDefault();

        if (property == null) return false;

        property.IsDeleted = false;
        property.DeletedAt = null;

        _unitOfWork.Properties.Update(property);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}