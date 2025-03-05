using Microsoft.Extensions.Logging;
using Sentry;

namespace StainManager.Infrastructure.Services.Sentry;

public interface IExceptionService
{
    void CaptureException(
        Exception exception,
        Dictionary<string, string>? additionalData = null);
}

public class SentryExceptionService(
    IHub sentryHub,
    ILogger<SentryExceptionService> logger) 
    : IExceptionService
{
    public void CaptureException(
        Exception exception,
        Dictionary<string, string>? additionalData = null)
    {
        sentryHub.ConfigureScope(scope =>
        {
            scope.SetTag("layer", "Infrastructure");

            if (additionalData is null) 
                return;
            
            foreach (var (key, value) in additionalData)
                scope.SetExtra(key, value);
        });
        
        var sentryId = sentryHub.CaptureException(exception);
        
        logger.LogError(exception, 
            "Exception captured (Sentry ID: {SentryId}). Additional data: {@AdditionalData}", 
            sentryId, 
            additionalData);
    }
}