using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace LibrarySystem.API.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> Upload(IFormFile file, [FromQuery] int? itemId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("لم يتم اختيار ملف للرفع.");

            // 1. Organized folder structure by itemId
            var baseFolder = Path.Combine(_environment.ContentRootPath, "uploads");
            var mediaFolder = itemId.HasValue
                ? Path.Combine(baseFolder, "items", itemId.Value.ToString(), "media")
                : Path.Combine(baseFolder, "general");

            if (!Directory.Exists(mediaFolder))
                Directory.CreateDirectory(mediaFolder);

            // 2. Generate unique file name
            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(mediaFolder, uniqueFileName);

            // 3. Save file to disk
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 4. Build storage path
            var storagePath = itemId.HasValue
                ? $"/uploads/items/{itemId.Value}/media/{uniqueFileName}"
                : $"/uploads/general/{uniqueFileName}";

            // 5. Extract image metadata if it's an image
            int? width = null;
            int? height = null;
            string? thumbnailPath = null;

            var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (imageExtensions.Contains(extension))
            {
                using var image = await Image.LoadAsync(filePath);
                width = image.Width;
                height = image.Height;

                // 6. Generate thumbnail
                var thumbnailFolder = itemId.HasValue
                    ? Path.Combine(baseFolder, "items", itemId.Value.ToString(), "thumbnails")
                    : Path.Combine(baseFolder, "general", "thumbnails");

                if (!Directory.Exists(thumbnailFolder))
                    Directory.CreateDirectory(thumbnailFolder);

                var thumbnailFileName = $"thumb_{uniqueFileName}";
                var thumbnailFilePath = Path.Combine(thumbnailFolder, thumbnailFileName);

                using var thumbnail = await Image.LoadAsync(filePath);
                thumbnail.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(200, 200),
                    Mode = ResizeMode.Max  // keeps aspect ratio
                }));
                await thumbnail.SaveAsync(thumbnailFilePath);

                thumbnailPath = itemId.HasValue
                    ? $"/uploads/items/{itemId.Value}/thumbnails/{thumbnailFileName}"
                    : $"/uploads/general/thumbnails/{thumbnailFileName}";
            }

            return Ok(new
            {
                StoragePath = storagePath,
                FileName = file.FileName,
                MimeType = file.ContentType,
                FileSize = file.Length,
                Width = width,        // null for non-images
                Height = height,       // null for non-images
                ThumbnailPath = thumbnailPath // null for non-images
            });
        }
    }
}