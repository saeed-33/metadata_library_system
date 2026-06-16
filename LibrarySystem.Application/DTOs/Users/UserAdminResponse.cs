namespace LibrarySystem.Application.DTOs.Users;

public record UserAdminResponse(
    int Id,
    string ExternalId,      // الربط مع AspNetUsers
    string FullName,
    string? Bio,
    string? ProfilePicturePath,
    bool IsDeleted ,// <--- الإضافة هنا

    List<string> Roles      // أسماء الأدوار فقط للعرض
);