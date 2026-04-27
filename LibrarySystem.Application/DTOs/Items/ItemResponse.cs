namespace LibrarySystem.Application.DTOs.Items;

public record ItemResponse(
    int Id,
    string Type,
    int? TemplateId,
    int? OwnerId,
    string? OwnerName,
    List<ItemValueResponse> MetadataValues // قائمة القيم الوصفية
);

public record ItemValueResponse(
    int PropertyId,
    string PropertyLabel,
    string? ValueText,
    string? Language
);