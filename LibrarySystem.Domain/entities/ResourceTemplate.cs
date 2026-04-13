using LibrarySystem.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Domain.Entities
{
    public class ResourceTemplate : BaseEntity
    {
        public string Label { get; set; } = null!;
        public string? Description { get; set; }
        public virtual ICollection<TemplateProperty> TemplateProperties { get; set; } = new List<TemplateProperty>();
    }
}