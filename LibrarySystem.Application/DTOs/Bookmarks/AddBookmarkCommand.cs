using MediatR;

namespace LibrarySystem.Application.DTOs.Bookmarks;

public record AddBookmarkCommand(
    string ExternalUserId, 
    int ItemId
    ) : IRequest<bool>;
