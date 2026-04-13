using LibrarySystem.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystem.Domain.Entities
{
    public class Value : BaseEntity
    {
        public int ResourceId { get; set; }
        public virtual Resource Resource { get; set; } = null!;
        public int PropertyId { get; set; }
        public virtual Property Property { get; set; } = null!;
        public string? ValueText { get; set; }
        public string? ValueUri { get; set; }
        public int? ValueResourceId { get; set; }
        public virtual Resource? LinkedResource { get; set; }
        public string Type { get; set; } = null!; // literal, uri, resource
        public string? Language { get; set; }
    }
}