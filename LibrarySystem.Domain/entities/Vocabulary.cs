using LibrarySystem.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Domain.Entities
{
    public class Vocabulary : BaseEntity
    {
        public string Prefix { get; set; } = null!;
        public string NamespaceUri { get; set; } = null!;
        public string Label { get; set; } = null!;
        public virtual ICollection<Property> Properties { get; set; } = new List<Property>();
    }
}