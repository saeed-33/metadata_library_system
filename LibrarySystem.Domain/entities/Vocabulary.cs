using LibrarySystem.Domain.Common;

namespace LibrarySystem.Domain.Entities
{
    public class Vocabulary : BaseEntity
    {
        public string Name { get; set; } // e.g., Dublin Core
        public string Prefix { get; set; } // e.g., dc
        public string NamespaceUri { get; set; }

        public ICollection<Property> Properties { get; set; } = new List<Property>();
    }
}