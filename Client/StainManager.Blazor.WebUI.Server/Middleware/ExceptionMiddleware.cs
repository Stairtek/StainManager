using System.Net;
using System.Text.Json;
using StainManager.Blazor.WebUI.Server.Infrastructure;
using StainManager.Blazor.WebUI.Server.Models;

namespace StainManager.Blazor.WebUI.Server.Middleware;

public class ExceptionMiddleware(
    RequestDelegate next,
    ILogger<ExceptionMiddleware> logger,
    IHub sentryHub)
{
    private const string CorrelationIdHeaderName = "X-Correlation-Id";
    
    public async Task InvokeAsync(
        HttpContext context,
        ISentryHandler sentryHandler)
    {
        var correlationId = GetOrCreateCorrelationId(context);
        
        var transactionName = $"{context.Request.Method} {context.Request.Path}";
        var transaction = sentryHub.StartTransaction(transactionName, "http.server");
        
        sentryHub.ConfigureScope(scope =>
        {
            scope.Transaction = transaction;
            scope.SetTag("correlation_id", correlationId.ToString());
            scope.SetTag("app_type", "blazor_server");
        });
        
        try
        {
            transaction.SetTag("http.method", context.Request.Method);
            transaction.SetTag("http.url", context.Request.Path);
            transaction.SetTag("correlation_id", correlationId.ToString());

            var span = transaction.StartChild("middleware.processing");

            try
            {
                await next(context);
                
                span.Finish(context.Response.StatusCode < 400
                    ? SpanStatus.Ok
                    : SpanStatus.UnknownError);
            }
            catch (Exception)
            {
                span.Finish(SpanStatus.UnknownError);
                throw;
            }
            
            transaction.Status = context.Response.StatusCode < 400
                ? SpanStatus.Ok
                : SpanStatus.UnknownError;
        }
        catch (Exception error)
        {
            var errorSpan = transaction.StartChild("error.handling");

            try
            {
                await HandleExceptionAsync(context, error, correlationId, sentryHandler);
            }
            finally
            {
                errorSpan.Finish(SpanStatus.InternalError);
            }
            
            transaction.Status = SpanStatus.InternalError;
        }
        finally
        {
            transaction.SetTag("http.status_code", context.Response.StatusCode.ToString());
            transaction.Finish();
        }
    }
    
    private static Guid GetOrCreateCorrelationId(HttpContext context)
    {
        var retrievedValue = context.Request.Headers
            .TryGetValue(CorrelationIdHeaderName, out var correlationIdValue);
        
        if (!retrievedValue)
            return CreateCorrelationId(context);
        
        var parsedValue = Guid.TryParse(correlationIdValue, out var parsedId);
        
        return parsedValue 
            ? parsedId 
            : CreateCorrelationId(context);
    }

    private async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        Guid correlationId,
        ISentryHandler sentryHandler)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)GetStatusCodeForException(exception);
        
        context.Response.Headers[CorrelationIdHeaderName] = correlationId.ToString();
            
        var response = Result.Fail(
            "An unexpected error occurred", 
            new ExceptionResult
            {
                Id = correlationId,
                Type = exception.GetType().Name,
                ErrorMessage = exception.Message,
                DisplayMessage = "An unexpected error occurred",
                StackTrace = exception.StackTrace,
                Source = exception.Source
            });
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
            
        logger.LogError(
            exception,
            "An unexpected error occurred. Message: {Message}, Path: {Path}, CorrelationId: {CorrelationId}",
            exception.Message, context.Request.Path, correlationId);
            
        await sentryHandler.CaptureExceptionAsync(exception, exception.Message);
            
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }

    private static Guid CreateCorrelationId(HttpContext context)
    {
        var newId = Guid.NewGuid();
        context.Request.Headers[CorrelationIdHeaderName] = newId.ToString();
        return newId;
    }

    private HttpStatusCode GetStatusCodeForException(Exception exception)
    {
        return exception switch
        {
            ArgumentException => HttpStatusCode.BadRequest,
            KeyNotFoundException => HttpStatusCode.NotFound,
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            // Add more exception types as needed
            _ => HttpStatusCode.InternalServerError
        };
    }

}