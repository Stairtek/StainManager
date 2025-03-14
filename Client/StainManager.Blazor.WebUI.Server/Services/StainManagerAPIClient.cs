using StainManager.Blazor.WebUI.Server.Infrastructure;
using StainManager.Blazor.WebUI.Server.Models;

namespace StainManager.Blazor.WebUI.Server.Services;

public interface IStainManagerAPIClient
{
    Task<Result<T>> GetAsync<T>(string requestUri);
    
    Task<Result<T>> PostAsync<T>(string requestUri, T value);
    
    Task<Result<T>> PutAsync<T>(string requestUri, T value);
    
    Task<Result<bool>> PatchAsync<T>(string requestUri, T value);
    
    Task<Result<T>> DeleteAsync<T>(string requestUri);
}

public class StainManagerAPIClient(
    HttpClient httpClient,
    ILogger<StainManagerAPIClient> logger,
    ISentryHandler sentryHandler)
    : BaseAPIClient(logger, sentryHandler), IStainManagerAPIClient
{
    private async Task<Result<T>> ProcessRequestAsync<T>(
        HttpMethod method,
        string requestUri,
        object? content = null)
    {
        try
        {
            var request = new HttpRequestMessage(method, requestUri);
            
            if (content is not null)
                request.Content = JsonContent.Create(content);
            
            var correlationId = Guid.NewGuid();
            request.Headers.Add(CorrelationIdHeaderName, correlationId.ToString());
            logger.LogError("Client - Request - CorrelationId: {CorrelationId}", correlationId);
            
            var response = await httpClient.SendAsync(request);
            return await HandleResponse<T>(response);
        }
        catch (Exception error)
        {
            return Result.Fail<T>(error.Message);
        }
    }
    
    public async Task<Result<T>> GetAsync<T>(string requestUri)
        => await ProcessRequestAsync<T>(HttpMethod.Get, requestUri);

    public async Task<Result<T>> PostAsync<T>(string requestUri, T value)
        => await ProcessRequestAsync<T>(HttpMethod.Post, requestUri, value);

    public async Task<Result<T>> PutAsync<T>(string requestUri, T value)
        => await ProcessRequestAsync<T>(HttpMethod.Put, requestUri, value);

    public async Task<Result<bool>> PatchAsync<T>(string requestUri, T value)
        => await ProcessRequestAsync<bool>(HttpMethod.Patch, requestUri, value);

    public async Task<Result<T>> DeleteAsync<T>(string requestUri)
        => await ProcessRequestAsync<T>(HttpMethod.Delete, requestUri);
}