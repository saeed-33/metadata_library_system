using LibrarySystem.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Domain.Entities
{
    public class Vocabulary : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty; // Required

        [Required]
        [StringLength(20)]
        public string Prefix { get; set; } = string.Empty; // Required

        [Required]
        [StringLength(255)]
        public string NamespaceUri { get; set; } = string.Empty; // Required

        // Navigation property: Can be null if not "Included" in the query
        public virtual ICollection<Property>? Properties { get; set; }
    }
}