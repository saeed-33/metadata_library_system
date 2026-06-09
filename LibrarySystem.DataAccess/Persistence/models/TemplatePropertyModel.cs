using LibrarySystem.Domain.common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.DataAccess.Persistence.models
{
    [Table("TemplateProperties")]
    public class TemplatePropertyModel : ISoftDelete
    {
        public int TemplateId { get; set; }
        [ForeignKey("TemplateId")]
        public ResourceTemplateModel? Template { get; set; }

        public int PropertyId { get; set; }
        [ForeignKey("PropertyId")]
        public PropertyModel? Property { get; set; }

        public bool IsRequired { get; set; } = false;

        public int DisplayOrder { get; set; } = 0;

        [StringLength(100)]
        public string? AlternateLabel { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
