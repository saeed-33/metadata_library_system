using LibrarySystem.Application.DTOs.Items;

namespace LibrarySystem.Application.DTOs.Media;

public class MediaResponse
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public string StoragePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public List<ItemValueResponse> MetadataValues { get; set; } = new();
}