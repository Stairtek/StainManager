using System.Net;

namespace StainManager.WebAPI.Middleware;

public class ExceptionMiddleware(
    RequestDelegate next,
    ILogger<ExceptionMiddleware> logger,
    IHub sentryHub)
{
    private const string CorrelationIdHeaderName = "X-Correlation-Id";

    
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetOrCreateCorrelationId(context);
        
        var transactionName = $"{context.Request.Method} {context.Request.Path}";
        var transaction = sentryHub.StartTransaction(transactionName, "http.server");
        
        sentryHub.ConfigureScope(scope =>
        {
            scope.Transaction = transaction;
            scope.SetTag("correlation_id", correlationId.ToString());
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
                span.Finish(SpanStatus.InternalError);
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
                await HandleExceptionAsync(context, error, correlationId);
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

    private Guid GetOrCreateCorrelationId(HttpContext context)
    {
        foreach (var header in context.Request.Headers)
        {
            foreach (var value in header.Value)
                logger.LogInformation("Server - Request - Header: {Header} - Value: {Value}", header.Key, value);
        }
        
        var retrievedValue = context.Request.Headers
            .TryGetValue(CorrelationIdHeaderName, out var correlationIdValue);
        
        if (!retrievedValue)
            return CreateCorrelationId(context);
        
        var parsedValue = Guid.TryParse(correlationIdValue, out var parsedId);
        
        if (!parsedValue)
            return CreateCorrelationId(context);
        
        logger.LogError("Server - Request - CorrelationId found in request header: {CorrelationId}", parsedId);
        
        return parsedId;
    }

    private Guid CreateCorrelationId(HttpContext context)
    {
        var newId = Guid.NewGuid();
        
        logger.LogCritical("Server - Request - CorrelationId created: {CorrelationId}", newId);
        
        context.Request.Headers[CorrelationIdHeaderName] = newId.ToString();
        
        return newId;
    }

    
    private Task HandleExceptionAsync(
        HttpContext context, 
        Exception exception,
        Guid correlationId)
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
        
        sentryHub.CaptureException(exception);

        return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
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