using LibrarySystem.Domain.common;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.DataAccess.Persistence.models
{
    public class AppRoleModel : IdentityRole, ISoftDelete, IAuditable
    {
        [StringLength(500)]
        public string? Description { get; set; }

        // Logic to handle Soft Delete for roles if desired
        [Required] public DateTime CreatedAt { get; set; }
        [StringLength(128)] public string? CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        [StringLength(128)] public string? ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

    }
}
