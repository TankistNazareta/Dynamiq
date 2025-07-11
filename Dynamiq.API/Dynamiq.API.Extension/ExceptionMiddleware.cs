using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace Dynamiq.API.Extension
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode statusCode;
            string message;

            switch (exception)
            {
                case KeyNotFoundException ex:
                    statusCode = HttpStatusCode.NotFound;
                    message = ex.Message;
                    _logger.LogWarning(ex, "Not found.");
                    break;

                case UnauthorizedAccessException ex:
                    statusCode = HttpStatusCode.Unauthorized;
                    message = ex.Message;
                    _logger.LogWarning(ex, "Unauthorized.");
                    break;

                case TimeoutException:
                case InvalidOperationException:
                case ArgumentOutOfRangeException:
                case ArgumentException:
                case InvalidDataException:
                    statusCode = HttpStatusCode.BadRequest;
                    message = exception.Message;
                    _logger.LogWarning(exception, "Bad request.");
                    break;

                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    message = "Internal server error.";
                    _logger.LogError(exception, "Unhandled exception.");
                    break;
            }


            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var result = JsonSerializer.Serialize(new
            {
                error = message,
                statusCode = context.Response.StatusCode
            });

            await context.Response.WriteAsync(result);
        }
    }
}
