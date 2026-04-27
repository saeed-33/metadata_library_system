using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Commands.Media;

public record DeleteMediaCommand(int Id) : IRequest<bool>;

public class DeleteMediaCommandHandler : IRequestHandler<DeleteMediaCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public DeleteMediaCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(DeleteMediaCommand request, CancellationToken cancellationToken)
    {
        var media = await _unitOfWork.Medias.GetByIdAsync(request.Id);
        if (media == null) return false;

        _unitOfWork.Medias.Delete(media);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}