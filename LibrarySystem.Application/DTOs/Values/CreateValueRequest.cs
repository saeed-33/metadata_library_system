namespace LibrarySystem.Application.DTOs.Values;

public record CreateValueRequest(
    int PropertyId,
    string? ValueText,
    string? ValueUri,
    int? ValueResourceId,
    string Type = "literal",
    string Language = "ar"
);
