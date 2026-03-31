using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystem.Domain.Entities
{
    public class ItemSetMember
    {
        public int ItemId { get; set; }
        [ForeignKey("ItemId")]
        public virtual Item? Item { get; set; }

        public int ItemSetId { get; set; }
        [ForeignKey("ItemSetId")]
        public virtual ItemSet? ItemSet { get; set; }
    }
}