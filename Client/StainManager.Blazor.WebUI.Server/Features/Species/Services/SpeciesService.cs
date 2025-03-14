using MudBlazor;
using StainManager.Blazor.WebUI.Server.Features.Species.Models;
using StainManager.Blazor.WebUI.Server.Helpers;
using StainManager.Blazor.WebUI.Server.Models;
using StainManager.Blazor.WebUI.Server.Services;

namespace StainManager.Blazor.WebUI.Server.Features.Species.Services;

public interface ISpeciesService
{
    Task<List<SpeciesModel>?> GetSpeciesAsync(bool showDeleted = false);

    Task<Result<PaginatedList<SpeciesManagementModel>?>> GetSpeciesManagementAsync(string searchQuery,
        int pageNumber,
        int pageSize,
        bool showDeleted,
        SortDefinition<SpeciesManagementModel>? sortDefinition = null,
        ICollection<IFilterDefinition<SpeciesManagementModel>>? filterDefinitions = null);

    Task<Result<SpeciesModel>> GetSpeciesByIdAsync(int id);

    Task<Result<SpeciesModel>?> CreateSpeciesAsync(SpeciesModel species);

    Task<Result<SpeciesModel>?> UpdateSpeciesAsync(SpeciesModel species);

    Task<Result<bool>?> DeleteSpeciesAsync(int id);

    Task<Result<bool>?> RestoreSpeciesAsync(int id);
}

public class SpeciesService(
    IStainManagerAPIClient stainManagerAPIClient) 
    : ISpeciesService
{
    private readonly string _baseUrl = "species";

    public async Task<List<SpeciesModel>?> GetSpeciesAsync(
        bool showDeleted = false)
    {
        return await stainManagerAPIClient.GetAsync<List<SpeciesModel>>(
            $"{_baseUrl}?isActive={!showDeleted}");
    }

    public async Task<Result<PaginatedList<SpeciesManagementModel>?>> GetSpeciesManagementAsync(
        string searchQuery,
        int pageNumber,
        int pageSize,
        bool showDeleted,
        SortDefinition<SpeciesManagementModel>? sortDefinition = null,
        ICollection<IFilterDefinition<SpeciesManagementModel>>? filterDefinitions = null)
    {
        var query = ManagementQueryHelpers.BuildQueryString(
            searchQuery,
            pageNumber,
            pageSize,
            showDeleted,
            sortDefinition,
            filterDefinitions);
        
        return await stainManagerAPIClient.GetAsync<PaginatedList<SpeciesManagementModel>?>(
            $"{_baseUrl}/management?{query}");
    }

    public async Task<Result<SpeciesModel>> GetSpeciesByIdAsync(int id)
    {
        return await stainManagerAPIClient.GetAsync<SpeciesModel>(
            $"{_baseUrl}/{id}");
    }

    public async Task<Result<SpeciesModel>?> CreateSpeciesAsync(SpeciesModel species)
    {
        return await stainManagerAPIClient.PostAsync($"{_baseUrl}", species);
    }

    public async Task<Result<SpeciesModel>?> UpdateSpeciesAsync(SpeciesModel species)
    {
        return await stainManagerAPIClient.PutAsync($"{_baseUrl}/{species.Id}", species);
    }

    public async Task<Result<bool>?> DeleteSpeciesAsync(int id)
    {
        return await stainManagerAPIClient.DeleteAsync<bool>($"{_baseUrl}/{id}");
    }

    public async Task<Result<bool>?> RestoreSpeciesAsync(int id)
    {
        return await stainManagerAPIClient.PatchAsync($"{_baseUrl}/{id}/restore", false);
    }
}