using MediatR;

namespace LibrarySystem.Application.DTOs.Bookmarks;

public record RemoveBookmarkCommand(
    string ExternalUserId,
    int ItemId
    ) : IRequest<bool>;
