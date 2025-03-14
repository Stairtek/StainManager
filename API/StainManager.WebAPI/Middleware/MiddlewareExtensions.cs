namespace StainManager.WebAPI.Middleware;

public static class MiddlewareExtensions
{
    /// <summary>
    /// Adds the global exception handler middleware to the application's request pipeline.
    /// </summary>
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionMiddleware>();
    }

    /// <summary>
    /// Adds the request logging middleware to the application's request pipeline.
    /// </summary>
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestLoggingMiddleware>();
    }
}