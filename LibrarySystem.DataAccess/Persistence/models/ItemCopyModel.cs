using LibrarySystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystem.DataAccess.Persistence.models
{
    [Table("ItemCopies")]
    public class ItemCopyModel : BasePersistenceModel
    {
        [Required]
        public int ItemId { get; set; }
        public ItemModel Item { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Barcode { get; set; } = string.Empty;

        [Required]
        public ItemCopyStatus Status { get; set; } = ItemCopyStatus.Available;

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
