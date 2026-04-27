namespace LibrarySystem.Application.DTOs.Users;

public record UserResponse(
    int Id,
    string ExternalId,      // الربط مع AspNetUsers
    string FullName,
    string? Bio,
    string? ProfilePicturePath,
    List<string> Roles      // أسماء الأدوار فقط للعرض
);