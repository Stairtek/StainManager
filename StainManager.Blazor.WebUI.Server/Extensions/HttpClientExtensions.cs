using System.Text.Json;
using StainManager.Blazor.WebUI.Server.Common.Models;

namespace StainManager.Blazor.WebUI.Server.Extensions;

public static class HttpClientExtensions
{
    private static readonly JsonSerializerOptions DefaultJsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static async Task<Result<T>> HandleResponse<T>(
        HttpResponseMessage response,
        ILogger logger)
    {
        var responseString = await response.Content.ReadAsStringAsync();
        
        if (response.IsSuccessStatusCode)
        {
            logger.LogInformation(responseString);
            var result = JsonSerializer.Deserialize<Result<T>>(responseString, DefaultJsonOptions);
            return result ?? Result.Fail<T>("Failed to deserialize response");
        }
        
        var errorResult = JsonSerializer.Deserialize<Result<T>>(responseString, DefaultJsonOptions);

        if (errorResult is null || string.IsNullOrEmpty(errorResult.Error))
        {
            logger.LogError("Failed to deserialize error response");
            return Result.Fail<T>("Failed to get response");
        }

        if (errorResult.HandledError)
            logger.LogWarning(responseString);
        else
            logger.LogError(responseString);

        return Result.Fail<T>(errorResult.Error, errorResult.HandledError);
    }
    
    
    public static async Task<Result<T>> GetAsync<T>(
        this HttpClient httpClient, 
        string requestUri,
        ILogger logger)
    {
        try
        {
            var response = await httpClient.GetAsync(requestUri);
            return await HandleResponse<T>(response, logger);
        }
        catch (Exception error)
        {
            return Result.Fail<T>(error.Message);
        }
    }
    
    public static async Task<Result<T>> PostAsync<T>(
        this HttpClient httpClient, 
        string requestUri,
        T value,
        ILogger logger)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync(requestUri, value);
            return await HandleResponse<T>(response, logger);
        }
        catch (Exception error)
        {
            return Result.Fail<T>(error.Message);
        }
    }
    
    public static async Task<Result<T>> PutAsync<T>(
        this HttpClient httpClient, 
        string requestUri, 
        T value,
        ILogger logger)
    {
        try
        {
            var response = await httpClient.PutAsJsonAsync(requestUri, value);
            return await HandleResponse<T>(response, logger);
        }
        catch (Exception error)
        {
            return Result.Fail<T>(error.Message);
        }
    }
    
    public static async Task<Result<bool>> PatchAsync<T>(
        this HttpClient httpClient, 
        string requestUri, 
        T value,
        ILogger logger)
    {
        try
        {
            var response = await httpClient.PatchAsJsonAsync(requestUri, value);
            return await HandleResponse<bool>(response, logger);
        }
        catch (Exception error)
        {
            return Result.Fail<bool>(error.Message);
        }
    }
    
    public static async Task<Result<T>> DeleteAsync<T>(
        this HttpClient httpClient, 
        string requestUri,
        ILogger logger)
    {
        try
        {
            var response = await httpClient.DeleteAsync(requestUri);
            return await HandleResponse<T>(response, logger);
        }
        catch (Exception error)
        {
            return Result.Fail<T>(error.Message);
        }
    }
}