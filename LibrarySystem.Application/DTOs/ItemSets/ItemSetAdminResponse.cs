namespace LibrarySystem.Application.DTOs.ItemSets;

public class ItemSetAdminResponse
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public int? OwnerId { get; set; }
    public string? OwnerName { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPublic { get; set; }
    public List<ItemSetItemAdminResponse> Items { get; set; } = new();
    public bool IsDeleted { get; set; } // <--- الإضافة هنا

}

public class ItemSetItemAdminResponse
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public int? TemplateId { get; set; }
    public int? OwnerId { get; set; }
    public bool IsDeleted { get; set; } // <--- الإضافة هنا

}