using LibrarySystem.DataAccess.Persistence.models;
using Microsoft.AspNetCore.Identity;

namespace LibrarySystem.DataAccess.Persistence.Seeds
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(RoleManager<AppRoleModel> roleManager)
        {
            string[] roles = { "Admin", "Librarian", "User" };

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
