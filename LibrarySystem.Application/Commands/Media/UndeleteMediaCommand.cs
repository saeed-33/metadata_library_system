using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Commands.Media;

public record UndeleteMediaCommand(int Id) : IRequest<bool>;

public class UndeleteMediaCommandHandler : IRequestHandler<UndeleteMediaCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UndeleteMediaCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(UndeleteMediaCommand request, CancellationToken cancellationToken)
    {
        var mediaList = await _unitOfWork.Medias.FindWithDeletedAsync(m => m.Id == request.Id);
        var media = mediaList.FirstOrDefault();

        if (media == null) return false;

        media.IsDeleted = false;
        media.DeletedAt = null;

        _unitOfWork.Medias.Update(media);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}