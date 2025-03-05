namespace StainManager.WebAPI.Middleware;

public class GlobalExceptionHandlerMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionHandlerMiddleware> logger,
    IWebHostEnvironment environment,
    IHub sentryHub)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var transactionName = $"{context.Request.Method} {context.Request.Path}";
        var transaction = sentryHub.StartTransaction(transactionName, "http.server");
        
        sentryHub.ConfigureScope(scope => scope.Transaction = transaction);

        try
        {
            transaction.SetTag("http.method", context.Request.Method);
            transaction.SetTag("http.url", context.Request.Path);

            var span = transaction.StartChild("middleware.processing");

            try
            {
                await next(context);

                span.Finish(context.Response.StatusCode < 400 ? SpanStatus.Ok : SpanStatus.UnknownError);
            }
            catch (Exception)
            {
                span.Finish(SpanStatus.InternalError);
                throw;
            }

            transaction.Status = context.Response.StatusCode < 400 ? SpanStatus.Ok : SpanStatus.UnknownError;
        }
        catch (Exception error)
        {
            var errorSpan = transaction.StartChild("error.handling");

            try
            {
                await HandleExceptionAsync(context, error);
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

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var response = new
        {
            StatusCode = context.Response.StatusCode,
            Message = "An internal server error has occurred.",
            Detail = environment.IsDevelopment() ? exception.ToString() : null,
            Path = context.Request.Path,
            Timestamp = DateTime.UtcNow
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        logger.LogError(exception, "Unhandled exception occurred. Path: {Path}", context.Request.Path);
        SentrySdk.CaptureException(exception);
        
        return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}