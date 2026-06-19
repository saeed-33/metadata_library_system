using LibrarySystem.Domain.Common;

namespace LibrarySystem.Domain.Entities
{
   public class ItemCopy : BaseEntity
{
    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;

    public string Barcode { get; set; } = string.Empty; // رقم فريد يوضع خلف الكتاب
    
    // 0: Available, 1: Borrowed, 2: ReferenceOnly, 3: Maintenance
    public int Status { get; set; } 
    public string? Notes { get; set; } 
}
}