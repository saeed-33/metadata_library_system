using LibrarySystem.Application.DTOs.Users;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Queries.Bookmarks;

public record GetUserBookmarksQuery(string ExternalUserId) : IRequest<IEnumerable<BookmarksResponse>>;

public class GetUserBookmarksQueryHandler : IRequestHandler<GetUserBookmarksQuery, IEnumerable<BookmarksResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetUserBookmarksQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IEnumerable<BookmarksResponse>> Handle(GetUserBookmarksQuery request, CancellationToken cancellationToken)
    {
        var bookmarks = await _unitOfWork.Bookmarks.FindAsync(b => b.UserId == request.ExternalUserId);

        return bookmarks.Select(b => new BookmarksResponse(b.ItemId)).ToList();
    }
}