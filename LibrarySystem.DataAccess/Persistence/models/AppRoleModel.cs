using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.DataAccess.Persistence.models
{
    public class AppRoleModel : IdentityRole
    {
        [StringLength(500)]
        public string? Description { get; set; }

        // Logic to handle Soft Delete for roles if desired
        public bool IsDeleted { get; set; }
    }
}
