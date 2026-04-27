using LibrarySystem.Application.DTOs.Items;

namespace LibrarySystem.Application.DTOs.Media;

public record MediaResponse(
    int Id,
    int ItemId,               // المورد الأب (الكتاب التابع له)
    string StoragePath,       // مسار الملف على الخادم أو السحابة
    string FileName,          // اسم الملف الأصلي
    List<ItemValueResponse> MetadataValues // القيم الوصفية للملف نفسه
);