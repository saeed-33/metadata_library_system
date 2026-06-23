using LibrarySystem.Domain.Common;
using LibrarySystem.Domain.Enums;

namespace LibrarySystem.Domain.Entities
{
    public class BorrowRecord : BaseEntity
    {
        public int CopyId { get; set; }
        public ItemCopy Copy { get; set; } = null!;

        public int PatronId { get; set; }
        public Patron Patron { get; set; } = null!;

        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public BorrowRecordStatus Status { get; set; } = BorrowRecordStatus.Active;

        // الحالة الأصلية للنسخة قبل الإعارة، تُستخدم عند الإرجاع لاستعادة الحالة الصحيحة
        public ItemCopyStatus OriginalCopyStatus { get; set; } = ItemCopyStatus.Available;
    }
}
