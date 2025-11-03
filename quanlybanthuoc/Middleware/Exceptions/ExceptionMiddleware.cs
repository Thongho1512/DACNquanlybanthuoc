using quanlybanthuoc.Dtos;
using System.Text.Json;

namespace quanlybanthuoc.Middleware.Exceptions
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

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, message) = exception switch
            {
                NotFoundException notFoundEx =>
                    (StatusCodes.Status404NotFound, notFoundEx.Message),

                BadRequestException badRequestEx => 
                    (StatusCodes.Status400BadRequest, badRequestEx.Message),

                UnauthorizedException unauthorizedEx =>
                    (StatusCodes.Status401Unauthorized, unauthorizedEx.Message),

                _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")

            };

            context.Response.StatusCode = statusCode;

            var response = ApiResponse<string>.FailureResponse(message);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }
}