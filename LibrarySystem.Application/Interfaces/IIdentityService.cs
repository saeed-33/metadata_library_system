namespace LibrarySystem.Application.Interfaces;

public interface IIdentityService
{
    Task<List<string>> GetRolesByExternalIdAsync(string externalId);
    Task<bool> UpdateUserRolesAsync(string externalId, List<string> roleNames); // ← add
}