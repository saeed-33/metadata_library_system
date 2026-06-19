using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

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
    [MaxLength(20)]
    public string Status { get; set; } = "Active"; 
}
}
