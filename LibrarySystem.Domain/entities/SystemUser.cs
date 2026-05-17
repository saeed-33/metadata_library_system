using LibrarySystem.Domain.Common;
using LibrarySystem.Domain.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.Domain.Entities
{
    public class SystemUser : BaseEntity
    {
        // هذا الحقل هو الربط مع Identity (AspNetUsers)
        public string ExternalId { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty; // الحقل الذي كان مفقوداً
        public string? Bio { get; set; }
        public string? ProfilePicturePath { get; set; }

        public List<Role> Roles { get; set; } = new();

        // علاقة عكسية: الموارد التي يملكها هذا المستخدم
        public virtual ICollection<Resource> OwnedResources { get; set; } = new List<Resource>();
    }
}