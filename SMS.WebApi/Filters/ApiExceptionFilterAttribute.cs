using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SMS.Application.Exceptions;
using System.Net;

namespace SMS.WebApi.Filters
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            // --- 1. Handle Entity Not Found Exception (404) ---
            if (context.Exception is EntityNotFoundException notFoundException)
            {
                HandleNotFoundException(context, notFoundException);
            }
            // --- 2. Handle Validation Exception (400) ---
            // Thrown typically by the MediatR Validation Behavior pipeline
            else if (context.Exception is ValidationException validationException)
            {
                HandleValidationException(context, validationException);
            }
            // --- 3. Handle Bad Request Exception (400) ---
            // Thrown for business logic errors (e.g., email already exists)
            else if (context.Exception is BadRequestException badRequestException)
            {
                HandleBadRequestException(context, badRequestException);
            }
            // --- 4. Handle Forbidden Access Exception (403) ---
            else if (context.Exception is ForbiddenAccessException forbiddenException)
            {
                HandleForbiddenException(context, forbiddenException);
            }
            // --- 5. Catch-All for Unknown/Unexpected Exceptions (500) ---
            else
            {
                HandleUnknownException(context);
            }

            // Important: Call base.OnException to ensure other filters in the pipeline are executed.
            base.OnException(context);
        }

        // --- EXCEPTION HANDLING METHODS ---

        private static void HandleNotFoundException(ExceptionContext context, EntityNotFoundException exception)
        {
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;

            context.Result = new ObjectResult(new ProblemDetails
            {
                Status = (int)HttpStatusCode.NotFound,
                Title = "Resource Not Found",
                Detail = exception.Message
            });

            context.ExceptionHandled = true;
        }

        private static void HandleValidationException(ExceptionContext context, ValidationException exception)
        {
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            // ValidationProblemDetails is a standard ASP.NET Core object that serializes 
            // the validation errors into a standard format.
            context.Result = new ObjectResult(new ValidationProblemDetails(exception.Errors)
            {
                Status = (int)HttpStatusCode.BadRequest,
                Title = "Validation Failed",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            });

            context.ExceptionHandled = true;
        }

        private static void HandleBadRequestException(ExceptionContext context, BadRequestException exception)
        {
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            context.Result = new ObjectResult(new ProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Title = "Bad Request (Business Logic Error)",
                Detail = exception.Message
            });

            context.ExceptionHandled = true;
        }

        private static void HandleForbiddenException(ExceptionContext context, ForbiddenAccessException exception)
        {
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

            context.Result = new ObjectResult(new ProblemDetails
            {
                Status = (int)HttpStatusCode.Forbidden,
                Title = "Forbidden",
                Detail = exception.Message ?? "You do not have the necessary permissions for this action."
            });

            context.ExceptionHandled = true;
        }

        private static void HandleUnknownException(ExceptionContext context)
        {
            // Set a generic 500 status code for unhandled exceptions
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Log the exception details here for monitoring purposes.
            // logger.LogError(context.Exception, "An unhandled exception occurred.");

            context.Result = new ObjectResult(new ProblemDetails
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Title = "Internal Server Error",
                // Never expose technical details (like stack traces) in production!
                Detail = "An unexpected error occurred while processing the request."
            });

            context.ExceptionHandled = true;
        }
    }
}