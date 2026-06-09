using Logging.Application.DTOs;

namespace LoggingService.Application.IServices
{
    public interface ILogService
    {
        Task CreateLogAsync(CreateLogDto dto);
        public Task<IEnumerable<LogDto>> GetAllAsync();
    }

}
