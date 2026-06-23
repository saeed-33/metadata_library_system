using LibrarySystem.Domain.Common;
using LibrarySystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystem.Domain.Entities
{
    public class Property : BaseEntity
    {
        public int VocabularyId { get; set; }
        public virtual Vocabulary Vocabulary { get; set; } = null!;
        public string LocalName { get; set; } = null!;
        public string Label { get; set; } = null!;
        public string TermUri { get; set; } = null!; // link that define the meaning of this property

        public bool IsSearchable { get; set; } = false;
    }
}