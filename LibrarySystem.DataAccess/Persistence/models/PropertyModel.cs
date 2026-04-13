using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.DataAccess.Persistence.models
{
    [Table("Properties")]
    public class PropertyModel : BasePersistenceModel
    {
        public int VocabularyId { get; set; }
        [ForeignKey("VocabularyId")] public VocabularyModel? Vocabulary { get; set; }
        [Required, StringLength(100)] public string LocalName { get; set; } = string.Empty;
        [Required, StringLength(100)] public string Label { get; set; } = string.Empty;
        [Required, StringLength(255)] public string TermUri { get; set; } = string.Empty;
    }
}
