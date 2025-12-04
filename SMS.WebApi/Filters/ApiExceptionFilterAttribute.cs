using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using SMS.Application.Exceptions;
using SMS.Application.Services.Logging;
using System.Net;

namespace SMS.WebApi.Filters
{
    /// <summary>
    /// Centralized filter for handling common application exceptions and returning
    /// standardized RFC 7807 Problem Details results (JSON error responses).
    /// </summary>
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        // 1. Logger is now a private readonly INSTANCE member for thread safety
        private readonly IAppLogger<ApiExceptionFilterAttribute> _logger;

        public ApiExceptionFilterAttribute(IAppLogger<ApiExceptionFilterAttribute> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            // Reset ProblemDetails variable
            ProblemDetails problemDetails = null;

            // Use switch expression (C# 8+) for cleaner exception handling flow
            problemDetails = context.Exception switch
            {
                // --- 404: Not Found ---
                EntityNotFoundException notFoundEx => CreateProblemDetails(
                    HttpStatusCode.NotFound,
                    "Resource Not Found",
                    notFoundEx.Message),

                // --- 400: Bad Request (Business Logic) ---
                BadRequestException badRequestEx => CreateProblemDetails(
                    HttpStatusCode.BadRequest,
                    "Bad Request (Business Logic Error)",
                    badRequestEx.Message),

                // --- 403: Forbidden ---
                ForbiddenAccessException forbiddenEx => CreateProblemDetails(
                    HttpStatusCode.Forbidden,
                    "Forbidden",
                    forbiddenEx.Message ?? "You do not have the necessary permissions for this action."),

                // --- 409: Concurrency Conflict ---
                ConcurrencyException concurrencyEx => CreateProblemDetails(
                    HttpStatusCode.Conflict,
                    "Concurrency Conflict",
                    concurrencyEx.Message ?? "The record you were trying to update has been modified by another transaction."),

                // --- 409: Database Constraint Violation (e.g., Foreign Key) ---
                DbUpdateException dbUpdateEx => HandleDbUpdateException(context, dbUpdateEx),

                // --- 400: Validation Exception (Requires special handling - ValidationProblemDetails) ---
                FluentValidation.ValidationException validationEx => HandleValidationException(context, validationEx),

                // --- 500: Catch-All for Unknown/Unexpected Exceptions ---
                _ => HandleUnknownException(context)
            };

            // If the handler method (except Validation/Unknown) returned ProblemDetails, apply it.
            if (problemDetails != null)
            {
                context.HttpContext.Response.StatusCode = (int)problemDetails.Status.GetValueOrDefault((int)HttpStatusCode.InternalServerError);
                context.Result = new ObjectResult(problemDetails);
                context.ExceptionHandled = true;
            }

            // Important: Call base.OnException to ensure other filters in the pipeline are executed.
            base.OnException(context);
        }

        // --- HELPER METHODS ---

        /// <summary>
        /// Creates a standard ProblemDetails object (RFC 7807) for non-validation exceptions.
        /// </summary>
        private ProblemDetails CreateProblemDetails(HttpStatusCode status, string title, string detail, string type = null)
        {
            return new ProblemDetails
            {
                Status = (int)status,
                Title = title,
                Detail = detail,
                Type = type,
            };
        }

        /// <summary>
        /// Handles Entity Framework Core DbUpdateException, usually for foreign key or unique constraint violations.
        /// </summary>
        private ProblemDetails HandleDbUpdateException(ExceptionContext context, DbUpdateException exception)
        {
            // Log the full exception for server-side monitoring
            _logger.LogError(exception, "Database update exception encountered.");

            // Set response status code (409 Conflict is often suitable for constraint violations)
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;

            // IMPORTANT: Do NOT expose InnerException details to the client!
            return CreateProblemDetails(
                HttpStatusCode.Conflict,
                "Database Constraint Violation",
                "The requested action could not be completed because it would violate a database constraint. This resource may be referenced by other records."
            );
        }

        /// <summary>
        /// Handles the FluentValidation.ValidationException and returns a ValidationProblemDetails object.
        /// </summary>
        private ValidationProblemDetails HandleValidationException(ExceptionContext context, FluentValidation.ValidationException exception)
        {
            // Map the FluentValidation failures into the required dictionary format
            var errors = exception.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            // Create and return the ValidationProblemDetails object
            var problemDetails = new ValidationProblemDetails(errors)
            {
                Status = (int)HttpStatusCode.BadRequest,
                Title = "One or more validation errors occurred.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Instance = context.HttpContext.TraceIdentifier
            };

            context.Result = new ObjectResult(problemDetails);
            context.ExceptionHandled = true;

            // Return null or throw a return exception to avoid double handling in OnException switch
            return null;
        }

        /// <summary>
        /// Handles the final catch-all 500 exception. Logs the full exception details server-side.
        /// </summary>
        private ProblemDetails HandleUnknownException(ExceptionContext context)
        {
            // 2. Logging the full exception details is now CORRECTLY done using the instance logger
            _logger.LogError(context.Exception, "An unhandled exception occurred during request processing.");

            // Set a generic 500 status code for unhandled exceptions
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // IMPORTANT: Never expose technical details (like stack traces) in the ProblemDetails!
            return CreateProblemDetails(
                HttpStatusCode.InternalServerError,
                "Internal Server Error",
                "An unexpected error occurred while processing the request."
            );
        }
    }
}