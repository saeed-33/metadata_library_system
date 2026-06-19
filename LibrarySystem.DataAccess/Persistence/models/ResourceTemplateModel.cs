using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.DataAccess.Persistence.models
{
    [Table("ResourceTemplates")]
    public class ResourceTemplateModel : BasePersistenceModel
    {
        [Required]
        [StringLength(100)]
        public string Label { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsBorrowable { get; set; } = true;
        public int? DefaultBorrowDays { get; set; }

        public ICollection<TemplatePropertyModel> TemplateProperties { get; set; } = new List<TemplatePropertyModel>();
    }
}
