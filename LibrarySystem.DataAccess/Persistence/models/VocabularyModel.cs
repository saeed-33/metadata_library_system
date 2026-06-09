using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.DataAccess.Persistence.models
{
    [Table("Vocabularies")]
    public class VocabularyModel : BasePersistenceModel
    {
        [Required, StringLength(20)] public string Prefix { get; set; } = string.Empty;
        [Required, StringLength(255)] public string NamespaceUri { get; set; } = string.Empty;
        [Required, StringLength(100)] public string Label { get; set; } = string.Empty;
        public ICollection<PropertyModel> Properties { get; set; } = new List<PropertyModel>();
    }
}
