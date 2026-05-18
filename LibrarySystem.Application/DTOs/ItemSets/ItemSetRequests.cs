namespace LibrarySystem.Application.DTOs.ItemSets;

public record CreateItemSetRequest(
    string Title,
    string? Description,
    bool IsPublic = true,
    int? OwnerId = null
);

public record UpdateItemSetRequest(
    string Title,
    string? Description,
    bool IsPublic,
    int? OwnerId
);
