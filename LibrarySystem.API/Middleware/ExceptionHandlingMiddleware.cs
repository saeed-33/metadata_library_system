using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled API exception.");

            if (context.Response.HasStarted)
            {
                throw;
            }

            var (statusCode, title, detail) = MapException(exception);

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path
            };

            context.Response.StatusCode = problem.Status.Value;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(problem);
        }
    }

    private static (int StatusCode, string Title, string Detail) MapException(Exception exception)
    {
        return exception switch
        {
            ValidationException validationEx => (
                (int)HttpStatusCode.BadRequest,
                "Validation failed.",
                string.Join("; ", validationEx.Errors.Select(e => e.ErrorMessage))),

            InvalidOperationException invalidOpEx => (
                (int)HttpStatusCode.BadRequest,
                "Invalid operation.",
                invalidOpEx.Message),

            _ => (
                (int)HttpStatusCode.InternalServerError,
                "An unexpected error occurred.",
                "The request could not be completed.")
        };
    }
}
