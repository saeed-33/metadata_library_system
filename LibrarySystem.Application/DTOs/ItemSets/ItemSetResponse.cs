namespace LibrarySystem.Application.DTOs.ItemSets;

public class ItemSetResponse
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public int? OwnerId { get; set; }
    public string? OwnerName { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPublic { get; set; }
    public List<ItemSetItemResponse> Items { get; set; } = new();
}

public class ItemSetItemResponse
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public int? TemplateId { get; set; }
    public int? OwnerId { get; set; }
}