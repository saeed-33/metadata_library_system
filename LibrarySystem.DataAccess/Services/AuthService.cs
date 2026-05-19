using Google.Apis.Auth;
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
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                throw new InvalidOperationException("Email is already registered.");

            var user = new AppUserModel
            {
                UserName = request.UserName,
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException(errors);
            }

            await _userManager.AddToRoleAsync(user, SystemRoles.User);

            var token = await GenerateJwtTokenAsync(user);
            return new AuthResponse(user.UserName!, user.Email!, token);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password.");

            var result = await _signInManager
                .CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
                throw new UnauthorizedAccessException("Invalid email or password.");

            var token = await GenerateJwtTokenAsync(user);
            return new AuthResponse(user.UserName!, user.Email!, token);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<AuthResponse> LoginWithGoogleAsync(string idToken)
        {
            // 1. Ask Google to verify the token and give us the user's info
            var payload = await VerifyGoogleTokenAsync(idToken);

            // 2. Check if this Google user already has an account in our system
            var user = await _userManager.FindByEmailAsync(payload.Email);

            if (user == null)
            {
                // 3. First time login with Google — create an account automatically
                user = new AppUserModel
                {
                    // Google email is used as username since they have no password
                    UserName = payload.Email,
                    Email = payload.Email,
                    // Email is already verified by Google — no need for our own verification
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException(errors);
                }

                // 4. Assign default role — same as normal registration
                await _userManager.AddToRoleAsync(user, SystemRoles.User);
            }

            // 5. Generate our own JWT token — same process as normal login
            var token = await GenerateJwtTokenAsync(user);
            return new AuthResponse(user.UserName!, user.Email!, token);
        }

        // ── Private helpers ───────────────────────────────────────────

        private async Task<GoogleJsonWebSignature.Payload> VerifyGoogleTokenAsync(string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    // Your Google ClientId — only accept tokens issued for YOUR app
                    Audience = new[] { _configuration["Authentication:Google:ClientId"] }
                };

                // Send the token to Google's servers for verification
                // Returns the decoded user info (email, name, etc.) if valid
                // Throws an exception if the token is fake or expired
                return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            }
            catch
            {
                throw new UnauthorizedAccessException("Invalid Google token.");
            }
        }

        private async Task<string> GenerateJwtTokenAsync(AppUserModel user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name,           user.UserName!),
                new Claim(ClaimTypes.Email,          user.Email!)
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]
                    ?? throw new InvalidOperationException("Jwt:Key is missing.")));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}