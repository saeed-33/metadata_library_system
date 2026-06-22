namespace LibrarySystem.Application.DTOs.Properties;

public class PropertyAdminResponse
{
    public int Id { get; set; }
    public int VocabularyId { get; set; }
    public string VocabularyPrefix { get; set; } = string.Empty;
    public string LocalName { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string TermUri { get; set; } = string.Empty;
    public bool IsSearchable { get; set; }

    public bool IsDeleted { get; set; } // <--- الإضافة هنا

}