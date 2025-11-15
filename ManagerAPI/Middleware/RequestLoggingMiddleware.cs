using System.Security.Claims;

namespace ManagerAPI.Middleware
{
    // Middleware para registrar cada petición con datos del usuario.
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        //Loguea metodo, ruta, usuario y rol antes de procesar la peticion
        public async Task Invoke(HttpContext context)
        {
            var userId = context.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value ?? "anon";
            var role = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "anon";

            _logger.LogInformation($"[REQUEST] {context.Request.Method} {context.Request.Path} | User: {userId} | Role: {role}");

            await _next(context);
        }
    }
}
