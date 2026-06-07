using System.Net;
using Betrieb.Application.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Betrieb.API.Middleware;

/// Translates exceptions thrown deep in the Application layer (e.g. FluentValidation's
/// ValidationException from the MediatR pipeline) into proper ProblemDetails responses,
/// so handlers/validators don't need to know about HTTP status codes.
public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            await WriteProblemAsync(context, HttpStatusCode.BadRequest, new ValidationProblemDetails(errors)
            {
                Title = "One or more validation errors occurred.",
                Status = (int)HttpStatusCode.BadRequest,
            });
        }
        catch (NotFoundException ex)
        {
            await WriteProblemAsync(context, HttpStatusCode.NotFound, new ProblemDetails
            {
                Title = "Not Found",
                Detail = ex.Message,
                Status = (int)HttpStatusCode.NotFound,
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            await WriteProblemAsync(context, HttpStatusCode.Unauthorized, new ProblemDetails
            {
                Title = "Unauthorized",
                Detail = ex.Message,
                Status = (int)HttpStatusCode.Unauthorized,
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception while processing {Method} {Path}", context.Request.Method, context.Request.Path);

            await WriteProblemAsync(context, HttpStatusCode.InternalServerError, new ProblemDetails
            {
                Title = "An unexpected error occurred.",
                Status = (int)HttpStatusCode.InternalServerError,
            });
        }
    }

    private static Task WriteProblemAsync(HttpContext context, HttpStatusCode statusCode, ProblemDetails problem)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;
        return context.Response.WriteAsJsonAsync(problem);
    }
}
