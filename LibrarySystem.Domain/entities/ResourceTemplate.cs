using LibrarySystem.Domain.Common;

namespace LibrarySystem.Domain.Entities
{
    public class ResourceTemplate : BaseEntity
    {
        public string Label { get; set; } = null!;
        public string? Description { get; set; }
           public bool IsBorrowable { get; set; } = true; 
    public int? DefaultBorrowDays { get; set; } // إذا ترك فارغاً، سيأخذ من الإعدادات العامة
        public virtual ICollection<TemplateProperty> TemplateProperties { get; set; } = new List<TemplateProperty>();
    }
}