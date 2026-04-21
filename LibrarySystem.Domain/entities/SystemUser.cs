using LibrarySystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.Domain.Entities
{
    public class SystemUser : BaseEntity
    {
        // ... previous fields (FullName, Bio, etc.)

        public List<Role> Roles { get; set; } = new();
    }
}
