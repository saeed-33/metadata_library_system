using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.Application.DTOs.Auth
{
    public record AuthResponse(
        string UserName,
        string Email,
        string Token
    );
}