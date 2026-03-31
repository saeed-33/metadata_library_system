using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystem.Domain.Entities
{
    public class TemplateProperty
    {
        public int TemplateId { get; set; }
        [ForeignKey("TemplateId")]
        public virtual ResourceTemplate? Template { get; set; }

        public int PropertyId { get; set; }
        [ForeignKey("PropertyId")]
        public virtual Property? Property { get; set; }

        public bool IsRequired { get; set; }
        public int DisplayOrder { get; set; }
    }
}