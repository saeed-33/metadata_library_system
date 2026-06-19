using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

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
        [Range(0, 3)] // تحديد نطاق الحالات (متاح، معار، إلخ)
        public int Status { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
