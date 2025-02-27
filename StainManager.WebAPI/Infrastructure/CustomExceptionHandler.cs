using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StainManager.Application.Common.Exceptions;
using StainManager.Domain.Common;

namespace StainManager.WebAPI.Infrastructure;

public class CustomExceptionHandler
    : IExceptionHandler
{
    private readonly Dictionary<Type, Func<HttpContext, Exception, Task>> _exceptionHandlers;
    private readonly ILogger<CustomExceptionHandler> _logger;

    public CustomExceptionHandler(
        ILogger<CustomExceptionHandler> logger)
    {
        _logger = logger;
        
        // Register known exception types and handlers.
        _exceptionHandlers = new Dictionary<Type, Func<HttpContext, Exception, Task>>
        {
            { typeof(ValidationException), HandleValidationException },
            { typeof(NotFoundException), HandleNotFoundException },
            { typeof(Exception), HandleUnexpectedException }
            //{ typeof(UnauthorizedAccessException), HandleUnauthorizedAccessException },
            //{ typeof(ForbiddenAccessException), HandleForbiddenAccessException },
        };
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var exceptionType = exception.GetType();

        if (!_exceptionHandlers.TryGetValue(exceptionType, out var handler))
            return false;

        await handler.Invoke(httpContext, exception);

        return true;
    }

    private async Task HandleValidationException(HttpContext httpContext,
        Exception ex)
    {
        var exception = (ValidationException)ex;

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        
        var failResult = Result.Fail("One or more validation errors occurred.");
        
        _logger.LogError("Validation error: {Message}", exception.Message);
        
        await httpContext.Response.WriteAsJsonAsync(failResult);
    }

    private async Task HandleNotFoundException(HttpContext httpContext,
        Exception ex)
    {
        var exception = (NotFoundException)ex;

        httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
        
        var failResult = Result.Fail("The specified resource was not found.");
        
        _logger.LogError("Not found error: {Message}", exception.Message);
        
        await httpContext.Response.WriteAsJsonAsync(failResult);
    }
    
    private async Task HandleUnexpectedException(
        HttpContext httpContext,
        Exception ex)
    {
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var failResult = Result.Fail($"An unexpected error occurred. {ex.Message}");
        
        _logger.LogError("Unexpected error: {Message}", ex.Message);
        
        await httpContext.Response.WriteAsJsonAsync(failResult);
    }

    // private async Task HandleUnauthorizedAccessException(HttpContext httpContext, Exception ex)
    // {
    //     httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
    //
    //     await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
    //     {
    //         Status = StatusCodes.Status401Unauthorized,
    //         Title = "Unauthorized",
    //         Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
    //     });
    // }
    //
    // private async Task HandleForbiddenAccessException(HttpContext httpContext, Exception ex)
    // {
    //     httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
    //
    //     await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
    //     {
    //         Status = StatusCodes.Status403Forbidden,
    //         Title = "Forbidden",
    //         Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3"
    //     });
    // }
}