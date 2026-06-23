using LibrarySystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystem.DataAccess.Persistence.models
{
    [Table("BorrowRecords")]
    public class BorrowRecordModel : BasePersistenceModel
    {
        [Required]
        public int CopyId { get; set; }
        public ItemCopyModel Copy { get; set; } = null!;

        [Required]
        public int PatronId { get; set; }
        public PatronModel Patron { get; set; } = null!;

        [Required]
        public DateTime BorrowDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        [Required]
        public BorrowRecordStatus Status { get; set; } = BorrowRecordStatus.Active;

        [Required]
        public ItemCopyStatus OriginalCopyStatus { get; set; } = ItemCopyStatus.Available;
    }
}
