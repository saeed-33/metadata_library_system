using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.Domain.common
{
    public static class SystemRoles
    {
        public const string Admin = "Admin";         // Full system control
        public const string Librarian = "Librarian"; // Can manage items/vocabularies
        public const string Member = "Member";       // Can view and create personal sets
        public const string Guest = "Guest";         // Read only
    }
}
