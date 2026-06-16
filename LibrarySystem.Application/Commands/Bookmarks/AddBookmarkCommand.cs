using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.entities;
using LibrarySystem.Domain.Entities; // تأكد من مسار الكلاس
using MediatR;

namespace LibrarySystem.Application.Commands.Bookmarks;

public record AddBookmarkCommand(string ExternalUserId, int ItemId) : IRequest<bool>;

public class AddBookmarkCommandHandler : IRequestHandler<AddBookmarkCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public AddBookmarkCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(AddBookmarkCommand request, CancellationToken cancellationToken)
    {
        // 1. التأكد أن الـ Item موجود أصلاً في الداتابيز
        var itemExists = await _unitOfWork.Items.GetByIdAsync(request.ItemId);
        if (itemExists == null) return false;

        // 2. التحقق من عدم التكرار
        var existing = await _unitOfWork.Bookmarks.FindAsync(b => b.UserId == request.ExternalUserId && b.ItemId == request.ItemId);
        if (existing.Any()) return true; // مضاف مسبقاً

        // 3. الحفظ المباشر
        var bookmark = new Bookmark
        {
            UserId = request.ExternalUserId,
            ItemId = request.ItemId
        };

        await _unitOfWork.Bookmarks.AddAsync(bookmark);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}