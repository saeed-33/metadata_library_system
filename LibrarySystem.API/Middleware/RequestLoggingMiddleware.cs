using LibrarySystem.Application.Interfaces;
using System.Diagnostics;

namespace LibrarySystem.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ILoggingClient loggingClient)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();

                var user = context.User.Identity?.Name;
                var createdBy = string.IsNullOrWhiteSpace(user) ? "anonymous" : user;
                var message =
                    $"{context.Request.Method} {context.Request.Path} -> {context.Response.StatusCode} in {stopwatch.ElapsedMilliseconds}ms";

                try
                {
                    await loggingClient.LogAsync(message, createdBy);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to send request log to logging microservice.");
                }
            }
        }
    }
}
