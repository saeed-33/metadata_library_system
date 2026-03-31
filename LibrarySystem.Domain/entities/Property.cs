using LibrarySystem.Domain.Common;
using LibrarySystem.Domain.Enums;

namespace LibrarySystem.Domain.Entities
{
    public class Property : BaseEntity
    {
        public string Label { get; set; } // e.g., Title
        public string Description { get; set; }
        public PropertyDataType DataType { get; set; }

        public int VocabularyId { get; set; }
        public Vocabulary Vocabulary { get; set; }

        public ICollection<TemplateProperty> TemplateProperties { get; set; } = new List<TemplateProperty>();
    }
}