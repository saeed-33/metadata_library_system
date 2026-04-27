namespace LibrarySystem.Application.DTOs.ResourceTemplates;

public record ResourceTemplateResponse(
    int Id,
    string Label,
    string? Description,
    // قائمة الخصائص التي يحتوي عليها هذا القالب
    List<TemplatePropertyResponse> Properties
);

// DTO فرعي لتمثيل الخاصية داخل القالب
public record TemplatePropertyResponse(
    int PropertyId,
    string PropertyLabel,
    bool IsRequired,
    int DisplayOrder
);

/*القالب بحد ذاته بسيط(
 * Label, Description)،
 * ولكن عند عرضه، المستخدم يحتاج لرؤية قائمة الخصائص المرتبطة بهذا القالب(التي تأتي من جدول 
 * TemplateProperty).*/