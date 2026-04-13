using LibrarySystem.Domain.common;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystem.Domain.Entities
{
    public class TemplateProperty : ISoftDelete
    {
        public int TemplateId { get; set; }
        public virtual ResourceTemplate Template { get; set; } = null!;
        public int PropertyId { get; set; }
        public virtual Property Property { get; set; } = null!;
        public bool IsRequired { get; set; }
        public int DisplayOrder { get; set; }
        public string? AlternateLabel { get; set; }

        // Soft delete for junction table
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}