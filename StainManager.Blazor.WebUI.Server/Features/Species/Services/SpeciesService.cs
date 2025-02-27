using System.Text.Json;
using Mapster;
using MudBlazor;
using StainManager.Blazor.WebUI.Server.Common.Helpers;
using StainManager.Blazor.WebUI.Server.Common.Models;
using StainManager.Blazor.WebUI.Server.Features.Species.Models;
using StainManager.Blazor.WebUI.Server.Extensions;

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

    Task<Result<bool?>> DeleteSpeciesAsync(int id);

    Task<Result<bool>?> RestoreSpeciesAsync(int id);
}

public class SpeciesService(
    IHttpClientFactory httpClientFactory) 
    : ISpeciesService
{
    private readonly HttpClient _http = httpClientFactory.CreateClient("StainManagerAPI");
    private readonly string _baseUrl = "species";

    public async Task<List<SpeciesModel>?> GetSpeciesAsync(
        bool showDeleted = false)
    {
        return await _http.GetAsync<List<SpeciesModel>>($"{_baseUrl}?isActive={!showDeleted}");
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
        
        return await _http.GetAsync<PaginatedList<SpeciesManagementModel>?>($"{_baseUrl}/management?{query}");
    }

    public async Task<Result<SpeciesModel>> GetSpeciesByIdAsync(int id)
    {
        return await _http.GetAsync<SpeciesModel>($"{_baseUrl}/{id}");
    }

    public async Task<Result<SpeciesModel>?> CreateSpeciesAsync(SpeciesModel species)
    {
        return await _http.PostAsync($"{_baseUrl}", species);
    }

    public async Task<Result<SpeciesModel>?> UpdateSpeciesAsync(SpeciesModel species)
    {
        return await _http.PutAsync($"{_baseUrl}/{species.Id}", species);
    }

    public async Task<Result<bool?>> DeleteSpeciesAsync(int id)
    {
        return await _http.DeleteAsync<bool?>($"{_baseUrl}/{id}");
    }

    public async Task<Result<bool>?> RestoreSpeciesAsync(int id)
    {
        return await _http.PutAsync($"{_baseUrl}/{id}/restore", false);
    }
}