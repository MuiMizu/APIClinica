using System.Net;
using System.Text.Json;

namespace APIClinica.Middleware
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
                
                if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized && !context.Response.HasStarted)
                {
                    await HandleCustomManualError(context, "Token inválido o ausente.", (int)HttpStatusCode.Unauthorized);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            var statusCode = (int)HttpStatusCode.InternalServerError;
            var message = "Ha ocurrido un error inesperado en el servidor.";

            switch (exception)
            {
                case InvalidOperationException:
                    statusCode = (int)HttpStatusCode.Conflict;
                    message = exception.Message;
                    break;
                    
                case ArgumentException:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    message = exception.Message;
                    break;
                    
                case KeyNotFoundException:
                    statusCode = (int)HttpStatusCode.NotFound;
                    message = exception.Message;
                    break;
                    
                case UnauthorizedAccessException:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    message = "No tienes permiso para realizar esta acción.";
                    break;
            }

            context.Response.StatusCode = statusCode;

            var response = new
            {
                statusCode = context.Response.StatusCode,
                message = message
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);

            await context.Response.WriteAsync(json);
        }

        private async Task HandleCustomManualError(HttpContext context, string message, int statusCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = new
            {
                statusCode = statusCode,
                message = message
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }
    }
}
