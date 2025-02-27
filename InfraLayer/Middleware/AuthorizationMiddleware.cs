using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace InfraLayer.Middleware
{
    public class AuthorizationMiddleware
    {
            private readonly RequestDelegate _next;
            private readonly ILogger<AuthorizationMiddleware> _logger;

            public AuthorizationMiddleware(RequestDelegate next, ILogger<AuthorizationMiddleware> logger)
            {
                _next = next;
                _logger = logger;
            }

            public async Task InvokeAsync(HttpContext context)
            {
                await _next(context);
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    _logger.LogError("Not authorized");
                    context.Response.ContentType = "application/json";
                    var response = new
                    {
                        success = false,
                        message = "Unauthorized: No enough permissions."
                    };
                        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                }
               
            }
    }
}
