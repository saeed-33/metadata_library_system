using LibrarySystem.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Domain.Entities
{
    public class ResourceTemplate : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; } // Optional

        // Navigation Properties
        public virtual ICollection<TemplateProperty> TemplateProperties { get; set; } = new List<TemplateProperty>();
        public virtual ICollection<Item> Items { get; set; } = new List<Item>();
    }
}