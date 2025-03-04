namespace StainManager.WebAPI.Middleware;

public class RequestLoggingMiddleware(
    RequestDelegate next,
    ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Log the beginning of the request
        logger.LogInformation("Request {Method} {Path} started", 
            context.Request.Method, context.Request.Path);

        // Start timing the request
        var sw = System.Diagnostics.Stopwatch.StartNew();
        
        try
        {
            await next(context);
        }
        finally
        {
            sw.Stop();
            
            // Log completion with status code and timing
            logger.LogInformation(
                "Request {Method} {Path} completed with status code {StatusCode} in {ElapsedMs}ms", 
                context.Request.Method, 
                context.Request.Path, 
                context.Response.StatusCode,
                sw.ElapsedMilliseconds);
        }
    }
}