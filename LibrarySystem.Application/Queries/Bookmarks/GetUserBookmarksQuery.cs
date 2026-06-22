using AutoMapper;
using LibrarySystem.Application.DTOs.Bookmarks;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Queries.Bookmarks;

public record GetUserBookmarksQuery(string ExternalUserId) : IRequest<IEnumerable<BookmarksResponse>>;

public class GetUserBookmarksQueryHandler : IRequestHandler<GetUserBookmarksQuery, IEnumerable<BookmarksResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUserBookmarksQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BookmarksResponse>> Handle(GetUserBookmarksQuery request, CancellationToken cancellationToken)
    {
        var bookmarks = await _unitOfWork.Bookmarks.FindAsync(b => b.UserId == request.ExternalUserId);

        return _mapper.Map<List<BookmarksResponse>>(bookmarks);
    }
}
