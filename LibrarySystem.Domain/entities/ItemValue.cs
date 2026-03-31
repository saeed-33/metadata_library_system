using LibrarySystem.Domain.Common;

namespace LibrarySystem.Domain.Entities
{
    public class ItemValue : BaseEntity
    {
        public int ItemId { get; set; }
        public Item Item { get; set; }

        public int PropertyId { get; set; }
        public Property Property { get; set; }

        public string Value { get; set; } // The actual data
        public string LanguageCode { get; set; } // e.g., "ar", "en"

        // For internal linking to another Item
        public int? LinkedItemId { get; set; }
        public Item LinkedItem { get; set; }
    }
}