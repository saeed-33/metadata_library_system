using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Web.Controllers
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
            // 1. التحقق من وجود الملف
            if (file == null || file.Length == 0)
                return BadRequest("لم يتم اختيار ملف للرفع.");

            // 2. تحديد مسار التخزين (مجلد wwwroot/uploads)
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // 3. توليد اسم فريد للملف لمنع التكرار (Unique ID + Extension)
            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // 4. حفظ الملف فعلياً على القرص
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 5. إرجاع المسار النسبي الذي سيستخدمه الـ CreateMediaCommand لاحقاً
            // المسار سيكون مثل: /uploads/GUID_filename.jpg
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