using MudBlazor;
using StainManager.Blazor.WebUI.Server.Common.Helpers;
using StainManager.Blazor.WebUI.Server.Common.Interfaces;
using StainManager.Blazor.WebUI.Server.Common.Models;
using StainManager.Blazor.WebUI.Server.Extensions;
using StainManager.Blazor.WebUI.Server.Features.Textures.Models;

namespace StainManager.Blazor.WebUI.Server.Features.Textures.Services;

public interface ITexturesService : IWebAPIService
{
    Task<List<TextureModel>?> GetTexturesAsync(bool showDeleted = false);
    
    Task<Result<List<TextureSummaryModel>?>> GetTexturesSummaryAsync();
    
    Task<Result<PaginatedList<TextureManagementModel>?>> GetTexturesManagementAsync(
        string searchQuery,
        int pageNumber,
        int pageSize,
        bool showDeleted,
        SortDefinition<TextureManagementModel>? sortDefinition = null,
        ICollection<IFilterDefinition<TextureManagementModel>>? filterDefinitions = null);
    
    Task<Result<TextureModel>> GetTextureByIdAsync(int id);
    
    Task<Result<TextureModel>?> CreateTextureAsync(TextureModel texture);
    
    Task<Result<TextureModel>?> UpdateTextureAsync(TextureModel texture);

    Task<Result<bool>?> UpdateTexturesSortOrderAsync(
        List<DropItem> textures);
    
    Task<Result<bool>?> DeleteTextureAsync(int id);
    
    Task<Result<bool>?> RestoreTextureAsync(int id);
}


public class TexturesService(
    IHttpClientFactory httpClientFactory)
    : ITexturesService
{
    private readonly HttpClient _http = httpClientFactory.CreateClient("StainManagerAPI");
    private readonly string _baseUrl = "textures";
    
    public async Task<List<TextureModel>?> GetTexturesAsync(bool showDeleted = false)
    {
        return await _http.GetAsync<List<TextureModel>>($"{_baseUrl}?isActive={!showDeleted}");
    }
    
    public async Task<Result<List<TextureSummaryModel>?>> GetTexturesSummaryAsync()
    {
        return await _http.GetAsync<List<TextureSummaryModel>?>($"{_baseUrl}/summary");
    }

    public async Task<Result<PaginatedList<TextureManagementModel>?>> GetTexturesManagementAsync(
        string searchQuery,
        int pageNumber,
        int pageSize,
        bool showDeleted,
        SortDefinition<TextureManagementModel>? sortDefinition = null,
        ICollection<IFilterDefinition<TextureManagementModel>>? filterDefinitions = null)
    {
        var query = ManagementQueryHelpers.BuildQueryString(
            searchQuery,
            pageNumber,
            pageSize,
            showDeleted,
            sortDefinition,
            filterDefinitions);
        
        return await _http.GetAsync<PaginatedList<TextureManagementModel>?>($"{_baseUrl}/management?{query}");
    }

    public async Task<Result<TextureModel>> GetTextureByIdAsync(int id)
    {
        return await _http.GetAsync<TextureModel>($"{_baseUrl}/{id}");
    }

    public async Task<Result<TextureModel>?> CreateTextureAsync(TextureModel texture)
    {
        return await _http.PostAsync($"{_baseUrl}", texture);
    }

    public async Task<Result<TextureModel>?> UpdateTextureAsync(TextureModel texture)
    {
        return await _http.PutAsync($"{_baseUrl}/{texture.Id}", texture);
    }
    
    public async Task<Result<bool>?> UpdateTexturesSortOrderAsync(
        List<DropItem> textures)
    {
        return await _http.PatchAsync($"{_baseUrl}/sortorder", textures);
    }

    public async Task<Result<bool>?> DeleteTextureAsync(int id)
    {
        return await _http.DeleteAsync<bool>($"{_baseUrl}/{id}"); 
    }

    public async Task<Result<bool>?> RestoreTextureAsync(int id)
    {
        return await _http.PutAsync($"{_baseUrl}/{id}/restore", false); 
    }
}