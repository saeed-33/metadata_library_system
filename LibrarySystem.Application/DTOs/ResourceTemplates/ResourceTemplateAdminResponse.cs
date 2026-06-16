namespace LibrarySystem.Application.DTOs.ResourceTemplates;

public class ResourceTemplateAdminResponse
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<TemplatePropertyAdminResponse> Properties { get; set; } = new();
    public bool IsDeleted { get; set; } // <--- الإضافة هنا

}

public class TemplatePropertyAdminResponse
{
    public int PropertyId { get; set; }
    public string PropertyLabel { get; set; } = string.Empty; // needs setter
    public bool IsRequired { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsDeleted { get; set; } // <--- الإضافة هنا

}