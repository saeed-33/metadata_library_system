using LibrarySystem.DataAccess.Persistence.models;
using LibrarySystem.Domain.common;
using Microsoft.AspNetCore.Identity;

namespace LibrarySystem.DataAccess.Persistence.Seeds
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(RoleManager<AppRoleModel> roleManager)
        {
            string[] roles =
            {
                SystemRoles.Admin,
                SystemRoles.Librarian,
                SystemRoles.User,
                SystemRoles.Guest
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new AppRoleModel { Name = role });
                }
            }
        }
    }
}