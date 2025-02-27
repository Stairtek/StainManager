using System.Text.Json;
using StainManager.Blazor.WebUI.Server.Common.Models;

namespace StainManager.Blazor.WebUI.Server.Extensions;

public static class HttpClientExtensions
{
    private static readonly JsonSerializerOptions DefaultJsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static async Task<Result<T>> HandleResponse<T>(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<Result<T>>(DefaultJsonOptions);
            return result ?? Result.Fail<T>("Failed to deserialize response");
        }
        
        var errorResult = await response.Content.ReadFromJsonAsync<Result<T>>(DefaultJsonOptions);
        
        return errorResult is null 
            ? Result.Fail<T>("Failed to get response") 
            : Result.Fail<T>(errorResult.Error ?? "Failed to get response");
    }
    
    
    public static async Task<Result<T>> GetAsync<T>(
        this HttpClient httpClient, 
        string requestUri)
    {
        try
        {
            var response = await httpClient.GetAsync(requestUri);
            return await HandleResponse<T>(response);
        }
        catch (Exception error)
        {
            return Result.Fail<T>(error.Message);
        }
    }
    
    public static async Task<Result<T>> PostAsync<T>(
        this HttpClient httpClient, 
        string requestUri, 
        T value)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync(requestUri, value);
            return await HandleResponse<T>(response);
        }
        catch (Exception error)
        {
            return Result.Fail<T>(error.Message);
        }
    }
    
    public static async Task<Result<T>> PutAsync<T>(
        this HttpClient httpClient, 
        string requestUri, 
        T value)
    {
        try
        {
            var response = await httpClient.PutAsJsonAsync(requestUri, value);
            return await HandleResponse<T>(response);
        }
        catch (Exception error)
        {
            return Result.Fail<T>(error.Message);
        }
    }
    
    public static async Task<Result<T>> DeleteAsync<T>(
        this HttpClient httpClient, 
        string requestUri)
    {
        try
        {
            var response = await httpClient.DeleteAsync(requestUri);
            return await HandleResponse<T>(response);
        }
        catch (Exception error)
        {
            return Result.Fail<T>(error.Message);
        }
    }
}