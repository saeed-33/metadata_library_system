using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.DataAccess.Persistence.models
{
    [Table("SystemUsers")]
    public class SystemUserModel : BasePersistenceModel
    {
        [Required]
        public string ExternalId { get; set; } = string.Empty; // Link to Identity ID

        [Required]
        [StringLength(200)]
        public string FullName { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Bio { get; set; }

        [StringLength(500)]
        public string? ProfilePicturePath { get; set; }

        // Navigation property for Resources owned by this user
        public virtual ICollection<ResourceModel> OwnedResources { get; set; } = new List<ResourceModel>();
    }
}
