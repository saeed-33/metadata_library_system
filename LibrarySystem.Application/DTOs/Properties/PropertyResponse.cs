namespace LibrarySystem.Application.DTOs.Properties;

// استخدمنا record لأنه أخف في الذاكرة ومخصص لنقل البيانات فقط
public record PropertyResponse(
    int Id,
    int VocabularyId,
    string VocabularyPrefix, // مضاف لكي يظهر للمستخدم (مثلاً: "dc:Title")
    string LocalName,        // الاسم البرمجي (مثلاً: Title)
    string Label,            // الاسم المعروض (مثلاً: العنوان الرئيسي)
    string TermUri           // الرابط العالمي للخاصية
);