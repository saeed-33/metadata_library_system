using LibrarySystem.Domain.Common;

namespace LibrarySystem.Domain.Entities
{
   public class BorrowRecord : BaseEntity
{
    public int CopyId { get; set; }
    public ItemCopy Copy { get; set; } = null!;

    public int PatronId { get; set; }
    public Patron Patron { get; set; } = null!;

    
    public DateTime BorrowDate { get; set; } //When they actually take it 
    public DateTime DueDate { get; set; } //deadline
    public DateTime? ReturnDate { get; set; } //When they actually bring it back

        // حالات السجل: Active, Returned, Overdue
        public string Status { get; set; } = "Active"; 
}
}