using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Commands.Bookmarks;

public record RemoveBookmarkCommand(string ExternalUserId, int ItemId) : IRequest<bool>;

public class RemoveBookmarkCommandHandler : IRequestHandler<RemoveBookmarkCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public RemoveBookmarkCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(RemoveBookmarkCommand request, CancellationToken cancellationToken)
    {
        var existing = await _unitOfWork.Bookmarks.FindAsync(b => b.UserId == request.ExternalUserId && b.ItemId == request.ItemId);
        var bookmark = existing.FirstOrDefault();

        if (bookmark == null) return true; // غير موجود أصلاً

        _unitOfWork.Bookmarks.Delete(bookmark);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}