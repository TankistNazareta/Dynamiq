using Dynamiq.Application.CustomExceptions;
using Dynamiq.Application.DTOs;
using Dynamiq.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace Dynamiq.API.Middlewares
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
                case KeyNotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    message = exception.Message;
                    _logger.LogWarning(exception, "Not found.");
                    break;

                case UnauthorizedAccessException:
                    statusCode = HttpStatusCode.Unauthorized;
                    message = exception.Message;
                    _logger.LogWarning(exception, "Unauthorized.");
                    break;
                case EmailNotConfirmedException:
                case CartEmptyException:
                    statusCode = HttpStatusCode.Forbidden;
                    message = exception.Message;
                    _logger.LogWarning(exception, "Not confirmed Email.");
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

            var resDto = new ExceptionResponseDto(statusCode, message);

            var result = JsonSerializer.Serialize(resDto);

            await context.Response.WriteAsync(result);
        }
    }
}
