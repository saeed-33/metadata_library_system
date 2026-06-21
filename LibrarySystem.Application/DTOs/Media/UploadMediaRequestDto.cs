using Microsoft.AspNetCore.Http;

namespace LibrarySystem.Application.DTOs.Media;

public class UploadMediaRequestDto
{
    public IFormFile File { get; set; } = null!;
    public int ItemId { get; set; }
    public string ValuesJson { get; set; } = string.Empty;
}
