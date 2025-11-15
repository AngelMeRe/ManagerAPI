using System.Net;
using System.Text.Json;

namespace ManagerAPI.Middleware
{
    //Middleware para manejar excepciones globales y devolver errores controlados.

    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        // Intercepta la solicitud y captura errores no manejados.
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (UnauthorizedAccessException ex)
            {
                //Error cuando un usuario no tiene permisos suficientes
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                await context.Response.WriteAsJsonAsync(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                //Error general no controlado
                _logger.LogError(ex, "Unhandled error");

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var result = JsonSerializer.Serialize(new
                {
                    error = "Ocurrió un error inesperado",
                    detail = ex.Message
                });

                await context.Response.WriteAsync(result);
            }
        }
    }
}
