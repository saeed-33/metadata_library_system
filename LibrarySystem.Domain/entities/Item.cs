using LibrarySystem.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystem.Domain.Entities
{
    public class Item : BaseEntity
    {
        [Required]
        [StringLength(128)]
        public string CreatedBy { get; set; } = string.Empty;

        public int TemplateId { get; set; }
        [ForeignKey("TemplateId")]
        public virtual ResourceTemplate? Template { get; set; }

        // Navigation Properties
        public virtual ICollection<ItemValue> ItemValues { get; set; } = new List<ItemValue>();
        public virtual ICollection<MediaFile> MediaFiles { get; set; } = new List<MediaFile>();
        public virtual ICollection<ItemSetMember> ItemSetMembers { get; set; } = new List<ItemSetMember>();
    }
}