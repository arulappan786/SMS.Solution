using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SMS.Application.Services.Logging;
using System.Net;

namespace SMS.Infrastructure.Middlewares
{
    public class ExceptionHandlingMiddleware(IAppLogger<ExceptionHandlingMiddleware> logger) : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                // This will handle exceptions originating from EF Core operations
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Log the original exception for diagnostics
            logger.LogError(exception, "An unhandled database exception occurred.");

            // Default values for unhandled exceptions
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string title = "Internal Server Error";
            string detail = "An unexpected error occurred while accessing the database.";

            // --- Database-Specific Exception Handling ---
            if (exception is DbUpdateConcurrencyException)
            {
                // EF Core's native exception for optimistic concurrency conflicts (e.g., timestamp mismatch)
                statusCode = (int)HttpStatusCode.Conflict; // 409 Conflict
                title = "Concurrency Conflict";
                detail = "The record being updated has been modified by another user.";
                // Optionally throw your custom ConcurrencyException here for consistent internal logging
                // throw new ConcurrencyException(detail, exception); 
            }
            else if (exception is DbUpdateException dbUpdateException)
            {
                // Handles general database update failures (e.g., constraint violations, unique index errors)
                statusCode = (int)HttpStatusCode.BadRequest; // 400 Bad Request
                title = "Database Update Failed";
                detail = dbUpdateException.InnerException?.Message ?? "A database constraint was violated.";
                // Optionally throw your custom BadRequestException here
            }

            // Create a standardized ProblemDetails response
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail
            };

            return context.Response.WriteAsync(JsonConvert.SerializeObject(problemDetails));
        }
    }

    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
