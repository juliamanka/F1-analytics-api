using System.Data.Common;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace F1Analytics.Middlewares;

public class DatabaseErrorHandler
{
     private readonly RequestDelegate _next;
    private readonly ILogger<DatabaseErrorHandler> _logger;

    public DatabaseErrorHandler(RequestDelegate next, ILogger<DatabaseErrorHandler> logger)
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
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "A database update error occurred.");
            await HandleExceptionAsync(context, HttpStatusCode.BadRequest, "A database update error occurred.");
        }
        catch (DbException ex)
        {
            _logger.LogError(ex, "A database connection or command error occurred.");
            await HandleExceptionAsync(context, HttpStatusCode.ServiceUnavailable, "Database service is unavailable.");
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("A database operation was canceled.");
            await HandleExceptionAsync(context, HttpStatusCode.RequestTimeout, "Database operation was canceled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled error occurred.");
            await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var result = System.Text.Json.JsonSerializer.Serialize(new
        {
            error = message,
            statusCode = (int)statusCode
        });

        return context.Response.WriteAsync(result);
    }
}