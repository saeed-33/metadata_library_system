using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.DataAccess.Persistence.models
{
    public class AppUserModel : IdentityUser
    {
        // Link to the Library System User profile
        public int? SystemUserId { get; set; }

        [ForeignKey("SystemUserId")]
        public virtual SystemUserModel? SystemUser { get; set; }
    }
}
