namespace LibrarySystem.Application.DTOs.Items;

public class ItemAdminResponse
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public int? TemplateId { get; set; }
    public int? OwnerId { get; set; }
    public string? OwnerName { get; set; }
    public bool IsDeleted { get; set; } // <--- الإضافة هنا

    public List<ItemAdminValueResponse> MetadataValues { get; set; } = new();
}

public class ItemAdminValueResponse
{
    public int PropertyId { get; set; }
    public string PropertyLabel { get; set; } = string.Empty;
    public string? ValueText { get; set; }
    public string? Language { get; set; }
    public bool IsDeleted { get; set; } // <--- الإضافة هنا

}