using MudBlazor;
using StainManager.Blazor.WebUI.Server.Features.Textures.Models;
using StainManager.Blazor.WebUI.Server.Helpers;
using StainManager.Blazor.WebUI.Server.Infrastructure;
using StainManager.Blazor.WebUI.Server.Models;
using StainManager.Blazor.WebUI.Server.Services;

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
    IStainManagerAPIClient stainManagerAPIClient)
    : ITexturesService
{
    private readonly string _baseUrl = "textures";
    
    public async Task<List<TextureModel>?> GetTexturesAsync(bool showDeleted = false)
    {
        return await stainManagerAPIClient.GetAsync<List<TextureModel>>(
            $"{_baseUrl}?isActive={!showDeleted}");
    }
    
    public async Task<Result<List<TextureSummaryModel>?>> GetTexturesSummaryAsync()
    {
        return await stainManagerAPIClient.GetAsync<List<TextureSummaryModel>?>(
            $"{_baseUrl}/summary");
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
        
        return await stainManagerAPIClient.GetAsync<PaginatedList<TextureManagementModel>?>(
            $"{_baseUrl}/management?{query}");
    }

    public async Task<Result<TextureModel>> GetTextureByIdAsync(int id)
    {
        return await stainManagerAPIClient.GetAsync<TextureModel>($"{_baseUrl}/{id}");
    }

    public async Task<Result<TextureModel>?> CreateTextureAsync(TextureModel texture)
    {
        return await stainManagerAPIClient.PostAsync($"{_baseUrl}", texture);
    }

    public async Task<Result<TextureModel>?> UpdateTextureAsync(TextureModel texture)
    {
        return await stainManagerAPIClient.PutAsync($"{_baseUrl}/{texture.Id}", texture);
    }
    
    public async Task<Result<bool>?> UpdateTexturesSortOrderAsync(
        List<DropItem> textures)
    {
        return await stainManagerAPIClient.PatchAsync($"{_baseUrl}/sortorder", textures);
    }

    public async Task<Result<bool>?> DeleteTextureAsync(int id)
    {
        return await stainManagerAPIClient.DeleteAsync<bool>($"{_baseUrl}/{id}"); 
    }

    public async Task<Result<bool>?> RestoreTextureAsync(int id)
    {
        return await stainManagerAPIClient.PutAsync($"{_baseUrl}/{id}/restore", false); 
    }
}