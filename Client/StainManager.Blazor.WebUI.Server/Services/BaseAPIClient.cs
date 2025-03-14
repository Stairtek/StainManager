using System.Text.Json;
using StainManager.Blazor.WebUI.Server.Infrastructure;
using StainManager.Blazor.WebUI.Server.Models;

namespace StainManager.Blazor.WebUI.Server.Services;

public abstract class BaseAPIClient(
    ILogger logger,
    ISentryHandler sentryHandler)
{
    protected const string CorrelationIdHeaderName = "X-Correlation-Id";

    private static readonly JsonSerializerOptions DefaultJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    
    // Common response handling method
    protected async Task<Result<T>> HandleResponse<T>(
        HttpResponseMessage? response)
    {
        if (response is null)
        {
            const string? errorMessage = "No response received from the server";
            logger.LogError("{ErrorMessage}", errorMessage);
            await sentryHandler.CaptureExceptionAsync(errorMessage);
            return Result.Fail<T>(errorMessage);
        }

        string responseContent;

        try
        {
            responseContent = await response.Content.ReadAsStringAsync();
        }
        catch (Exception error)
        {
            const string errorMessage = "Failed to read response content";
            logger.LogError(error, "Failed to read response content");
            await sentryHandler.CaptureExceptionAsync(error, errorMessage);
            return Result.Fail<T>("Failed to read response content");
        }
        
        LogResponse(response, responseContent);
        
        return response.IsSuccessStatusCode 
            ? await DeserializeSuccessResponse<T>(responseContent) 
            : await DeserializeErrorResponse<T>(response, responseContent);
    }

    private void LogResponse(
        HttpResponseMessage response,
        string responseContent)
    {
        if (response.IsSuccessStatusCode)
        {
            logger.LogInformation("API Response: {Response}", responseContent);
            return;
        }
        
        logger.LogWarning("API Error Response: {Response}", responseContent);
    }
    
    private async Task<Result<T>> DeserializeSuccessResponse<T>(
        string responseContent)
    {
        try
        {
            var result = JsonSerializer.Deserialize<Result<T>>(responseContent, DefaultJsonOptions);
            return result ?? Result.Fail<T>("Failed to deserialize successful response");
        }
        catch (JsonException exception)
        {
            logger.LogError(exception, "Failed to deserialize successful response: {Content}", responseContent);
            await sentryHandler.CaptureExceptionAsync(exception, "Failed to deserialize successful response");
            return Result.Fail<T>("Error processing server response");
        }
    }

    private async Task<Result<T>> DeserializeErrorResponse<T>(
        HttpResponseMessage? response,
        string responseContent)
    {
        var correlationId = GetCorrelationId(response);
        
        try
        {
            var errorTResult = JsonSerializer.Deserialize<Result<T>>(responseContent, DefaultJsonOptions);
            
            if (errorTResult is not null)
            {
                LogError(errorTResult.HandledError, errorTResult.Error);

                if (!errorTResult.HandledError)
                    await sentryHandler.CaptureExceptionAsync(errorTResult.Error, correlationId);
                
                return Result.Fail<T>(errorTResult.Error, errorTResult.HandledError);
            }
        }
        catch (JsonException)
        {
            // Silently catch this exception, we'll try another format
        }

        try
        {
            var errorResult = JsonSerializer.Deserialize<Result<ExceptionResult>>(responseContent, DefaultJsonOptions);
            
            if (errorResult is not null && !string.IsNullOrEmpty(errorResult.Error))
            {
                LogError(errorResult.HandledError, errorResult.Error);
                await sentryHandler.CaptureExceptionAsync(errorResult.Value);
                return Result.Fail<T>(errorResult.Error, errorResult.HandledError);
            }
        }
        catch (JsonException exception)
        {
            logger.LogError(exception, "Failed to deserialize error response: {Content}", responseContent);
            await sentryHandler.CaptureExceptionAsync(exception, "Failed to deserialize error response");
        }
        
        logger.LogError("Failed to deserialize error response: {StatusCode}, {Content}", 
            response?.StatusCode, responseContent);
        return Result.Fail<T>($"Server returned an error: {response?.StatusCode}");
    }
    
    private Guid? GetCorrelationId(
        HttpResponseMessage? response)
    {
        if (response is null)
            return null;

        var correlationIdHeader = response.Headers.GetValues(CorrelationIdHeaderName).FirstOrDefault();
        var correlationId = Guid.TryParse(correlationIdHeader, out var guid) 
            ? guid 
            : (Guid?)null;
        
        logger.LogError("Client - Response - CorrelationId: {CorrelationId}", correlationId);

        return correlationId;
    }

    private void LogError(
        bool handledError,
        string? message)
    {
        if (handledError)
        {
            logger.LogWarning("An error occurred: {Message}", message);
            return;
        }

        logger.LogError("An error occurred: {Message}", message);
    }
}