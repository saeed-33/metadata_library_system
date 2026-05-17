namespace LibrarySystem.Application.DTOs.ItemSets;

public record ItemSetResponse(
    int Id,
    string Type,
    int? OwnerId,
    string? OwnerName,
    string Title,
    string? Description,
    bool IsPublic,
    List<ItemSetItemResponse> Items
);

public record ItemSetItemResponse(
    int Id,
    string Type,
    int? TemplateId,
    int? OwnerId
);
