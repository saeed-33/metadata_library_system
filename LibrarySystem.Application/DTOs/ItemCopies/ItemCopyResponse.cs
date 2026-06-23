using LibrarySystem.Domain.Enums;

namespace LibrarySystem.Application.DTOs.ItemCopies;

public class ItemCopyResponse
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public ItemCopyStatus Status { get; set; }
    public string? Notes { get; set; }
}
