using LibrarySystem.Domain.common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.DataAccess.Persistence.models
{
    public abstract class BasePersistenceModel :ISoftDelete
    {
        [Key] public int Id { get; set; }
        [Required] public DateTime CreatedAt { get; set; }
        [StringLength(128)] public string? CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        [StringLength(128)] public string? ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }

}
