using LibrarySystem.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystem.Domain.Entities
{
    public class ItemValue : BaseEntity
    {
        public int ItemId { get; set; }
        [ForeignKey("ItemId")]
        public virtual Item? Item { get; set; }

        public int PropertyId { get; set; }
        [ForeignKey("PropertyId")]
        public virtual Property? Property { get; set; }

        [Required]
        public string Value { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string LanguageCode { get; set; } = "en"; // Default value

        // Requirement 3-6: Internal Linking is OPTIONAL
        public int? LinkedItemId { get; set; } // "?" is required here!
        [ForeignKey("LinkedItemId")]
        public virtual Item? LinkedItem { get; set; }
    }
}