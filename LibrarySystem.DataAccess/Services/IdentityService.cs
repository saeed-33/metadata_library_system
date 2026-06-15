using LibrarySystem.Application.Interfaces;
using LibrarySystem.DataAccess.Persistence.models;
using Microsoft.AspNetCore.Identity;

namespace LibrarySystem.DataAccess.Services;

public class IdentityService : IIdentityService
{
	private readonly UserManager<AppUserModel> _userManager;

	public IdentityService(UserManager<AppUserModel> userManager)
	{
		_userManager = userManager;
	}

	public async Task<List<string>> GetRolesByExternalIdAsync(string externalId)
	{
		var user = await _userManager.FindByIdAsync(externalId);
		if (user == null) return new List<string>();
		return (await _userManager.GetRolesAsync(user)).ToList();
	}
    public async Task<bool> UpdateUserRolesAsync(string externalId, List<string> roleNames)
    {
        var user = await _userManager.FindByIdAsync(externalId);
        if (user == null) return false;

        // 1. Remove all current roles
        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);

        // 2. Assign new roles
        var result = await _userManager.AddToRolesAsync(user, roleNames);
        return result.Succeeded;
    }
}