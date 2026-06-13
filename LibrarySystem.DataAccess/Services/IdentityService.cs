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
}