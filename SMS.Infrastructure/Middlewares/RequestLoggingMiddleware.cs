using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using SMS.Application.Services.Logging;

namespace SMS.Infrastructure.Middlewares
{
    public class RequestLoggingMiddleware(IAppLogger<RequestLoggingMiddleware> logger) : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            logger.LogInfo($"Incoming: {context.Request.Method} {context.Request.Path}");
            await next(context);
        }
    }

    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLoggingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}
