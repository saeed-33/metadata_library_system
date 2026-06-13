namespace LibrarySystem.Application.Interfaces;

public interface IIdentityService
{
	Task<List<string>> GetRolesByExternalIdAsync(string externalId);
}