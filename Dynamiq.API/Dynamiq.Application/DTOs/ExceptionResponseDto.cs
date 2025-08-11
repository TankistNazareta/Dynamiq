using System.Net;

namespace Dynamiq.Application.DTOs
{
    public class ExceptionResponseDto
    {
        public HttpStatusCode StatusCode { get; private set; }
        public string Message { get; private set; }

        public ExceptionResponseDto(HttpStatusCode statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }

        private ExceptionResponseDto() { }
    }
}
