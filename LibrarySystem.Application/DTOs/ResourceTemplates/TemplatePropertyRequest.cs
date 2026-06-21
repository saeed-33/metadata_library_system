namespace LibrarySystem.Application.DTOs.ResourceTemplates;

public record TemplatePropertyRequest(
    int PropertyId,
    bool IsRequired,
    int DisplayOrder,
    string? AlternateLabel
);
