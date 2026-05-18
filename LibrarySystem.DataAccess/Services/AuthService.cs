using LibrarySystem.Application.DTOs.Auth;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.DataAccess.Persistence.models;
using LibrarySystem.Domain.common;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LibrarySystem.DataAccess.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly SignInManager<AppUserModel> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<AppUserModel> userManager,
            SignInManager<AppUserModel> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            // 1. Check if email already taken
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                throw new InvalidOperationException("Email is already registered.");

            // 2. Build the Identity user object
            var user = new AppUserModel
            {
                UserName = request.UserName,
                Email = request.Email
            };

            // 3. Create user — Identity hashes the password automatically
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException(errors);
            }

            // 4. Assign default role
            await _userManager.AddToRoleAsync(user, SystemRoles.User);

            // 5. Generate and return token
            var token = await GenerateJwtTokenAsync(user);
            return new AuthResponse(user.UserName!, user.Email!, token);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            // 1. Find user by email
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password.");

            // 2. Verify password without creating a cookie session
            var result = await _signInManager
                .CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
                throw new UnauthorizedAccessException("Invalid email or password.");

            // 3. Generate and return token
            var token = await GenerateJwtTokenAsync(user);
            return new AuthResponse(user.UserName!, user.Email!, token);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        // ── Private JWT helper ────────────────────────────────────────
        private async Task<string> GenerateJwtTokenAsync(AppUserModel user)
        {
            // 1. Get user roles to embed in token
            var roles = await _userManager.GetRolesAsync(user);

            // 2. Build claims — information embedded inside the token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name,           user.UserName!),
                new Claim(ClaimTypes.Email,          user.Email!)
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            // 3. Build signing key from appsettings.json
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]
                    ?? throw new InvalidOperationException("Jwt:Key is missing.")));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 4. Build the token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            // 5. Serialize token to string (the eyJ... format)
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}