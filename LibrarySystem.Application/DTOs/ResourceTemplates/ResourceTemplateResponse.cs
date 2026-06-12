namespace LibrarySystem.Application.DTOs.ResourceTemplates;

public class ResourceTemplateResponse
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<TemplatePropertyResponse> Properties { get; set; } = new();
}

public class TemplatePropertyResponse
{
    public int PropertyId { get; set; }
    public string PropertyLabel { get; set; } = string.Empty; // needs setter
    public bool IsRequired { get; set; }
    public int DisplayOrder { get; set; }
}