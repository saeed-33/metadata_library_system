using LibrarySystem.Domain.Common;

namespace LibrarySystem.Domain.Entities
{
    public class ResourceTemplate : BaseEntity
    {
        public string Name { get; set; } // e.g., Manuscript Template
        public string Description { get; set; }

        public ICollection<TemplateProperty> TemplateProperties { get; set; } = new List<TemplateProperty>();
        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
}