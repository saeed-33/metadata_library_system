using LibrarySystem.Application.Interfaces;
using MediatR;


namespace LibrarySystem.Application.Commands.Properties;

public record DeletePropertyCommand(int Id) : IRequest<bool>;

public class DeletePropertyCommandHandler : IRequestHandler<DeletePropertyCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeletePropertyCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(DeletePropertyCommand request, CancellationToken cancellationToken)
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(request.Id);
        if (property == null) return false;

        // استدعاء الحذف (سيتحول لـ Soft Delete بفضل الـ DbContext)
        _unitOfWork.Properties.Delete(property);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}