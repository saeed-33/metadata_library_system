using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.DataAccess.Persistence.models
{
    [Table("Patrons")]

    public class PatronModel : BasePersistenceModel
    {
       [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string NationalId { get; set; } = string.Empty; // رقم الهوية أو الجامعي

    [Required]
    [MaxLength(20)]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;

    [MaxLength(150)]
    [EmailAddress]
    public string? Email { get; set; }
}
}
