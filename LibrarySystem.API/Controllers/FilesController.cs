using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public FilesController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            // 1. Check file exists
            if (file == null || file.Length == 0)
                return BadRequest("لم يتم اختيار ملف للرفع.");

            // 2. Use ContentRootPath instead of WebRootPath
            // WebRootPath is null in API projects without wwwroot
            var uploadsFolder = Path.Combine(_environment.ContentRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // 3. Generate unique file name to prevent duplicates
            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // 4. Save file to disk
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 5. Return relative path for CreateMediaCommand
            var storagePath = $"/uploads/{uniqueFileName}";

            return Ok(new
            {
                StoragePath = storagePath,
                FileName = file.FileName,
                MimeType = file.ContentType,
                FileSize = file.Length
            });
        }
    }
}