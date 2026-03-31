using LibrarySystem.Domain.Common;
using LibrarySystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystem.Domain.Entities
{
    public class Property : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Label { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; } // "?" means it CAN be NULL in the DB

        public PropertyDataType DataType { get; set; } // Enums are value types, not nullable by default

        public int VocabularyId { get; set; }
        [ForeignKey("VocabularyId")]
        public virtual Vocabulary? Vocabulary { get; set; } // Optional navigation
    }
}