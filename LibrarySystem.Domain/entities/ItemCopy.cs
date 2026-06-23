using LibrarySystem.Domain.Common;
using LibrarySystem.Domain.Enums;

namespace LibrarySystem.Domain.Entities
{
   public class ItemCopy : BaseEntity
{
    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;

    public string Barcode { get; set; } = string.Empty; // رقم فريد يوضع خلف الكتاب
    
    public ItemCopyStatus Status { get; set; } = ItemCopyStatus.Available;
    public string? Notes { get; set; } 
}
}
