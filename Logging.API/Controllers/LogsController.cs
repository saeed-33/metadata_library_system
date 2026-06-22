using Logging.Application.DTOs;
using LoggingService.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logging.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // لا نضع [Authorize] هنا لكي نتمكن من تخصيص الصلاحيات لكل دالة
    public class LogsController : ControllerBase
    {
        private readonly ILogService _logService;

        public LogsController(ILogService logService)
        {
            _logService = logService;
        }

        // POST: يمكن للمشروع الرئيسي (LibrarySystem) أو الـ Frontend استدعاؤه لتسجيل خطأ
        [HttpPost]
        [AllowAnonymous] // أو يمكنك حمايته باستخدام API Key مخصص للاتصال بين المشاريع
        public async Task<IActionResult> Log(CreateLogDto dto)
        {
            await _logService.CreateLogAsync(dto);
            return Ok("Logged");
        }

        // GET: للإدمن فقط - لكي يرى تقارير النظام
        [HttpGet]
        [Authorize(Roles = "Admin")] // تأكد أن الإعدادات في Program.cs تطابق المشروع الرئيسي
        public async Task<IActionResult> GetLogs()
        {
            var logs = await _logService.GetAllAsync();
            return Ok(logs);
        }

        // GET: للتجربة 
        [HttpGet("test")]
        [AllowAnonymous]
        public IActionResult Test()
        {
            var testResult = new LogDto
            {
                Id = 1,
                Message = "Test message",
                CreatedBy = "Admin",
                CreatedAt = DateTime.UtcNow
            };

            return Ok(testResult);
        }
    }
}