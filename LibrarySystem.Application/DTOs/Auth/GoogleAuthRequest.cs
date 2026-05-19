using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.Application.DTOs.Auth
{
    public record GoogleAuthRequest(
        string IdToken  // The token Google gives the user after they sign in
    );
}