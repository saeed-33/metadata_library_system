using LibrarySystem.Application.DTOs.Items;

namespace LibrarySystem.Application.DTOs.Media;

public class MediaAdminResponse
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public string StoragePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public List<ItemAdminValueResponse> MetadataValues { get; set; } = new();
    public bool IsDeleted { get; set; } // <--- الإضافة هنا

}