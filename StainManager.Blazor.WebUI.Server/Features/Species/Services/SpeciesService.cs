using System.Text.Json;
using Mapster;
using MudBlazor;
using StainManager.Blazor.WebUI.Server.Common.Models;
using StainManager.Blazor.WebUI.Server.Features.Species.Models;
using StainManager.Blazor.WebUI.Server.Extensions;

namespace StainManager.Blazor.WebUI.Server.Features.Species.Services;

public interface ISpeciesService
{
    Task<List<SpeciesModel>?> GetSpecies(bool showDeleted = false);

    Task<Result<PaginatedList<SpeciesManagementModel>?>> GetSpeciesManagement(string searchQuery,
        int pageNumber,
        int pageSize,
        bool showDeleted,
        SortDefinition<SpeciesManagementModel>? sortDefinition = null,
        ICollection<IFilterDefinition<SpeciesManagementModel>>? filterDefinitions = null);

    Task<Result<SpeciesModel>> GetSpeciesById(int id);

    Task<Result<SpeciesModel>?> CreateSpecies(SpeciesModel species);

    Task<Result<SpeciesModel>?> UpdateSpecies(SpeciesModel species);

    Task<Result<bool>> DeleteSpecies(int id);

    Task<Result<bool>?> RestoreSpecies(int id);
}

public class SpeciesService(
    IHttpClientFactory httpClientFactory) 
    : ISpeciesService
{
    private readonly HttpClient _http = httpClientFactory.CreateClient("StainManagerAPI");
    private readonly string _baseUrl = "species";

    public async Task<List<SpeciesModel>?> GetSpecies(
        bool showDeleted = false)
    {
        return await _http.GetAsync<List<SpeciesModel>>($"{_baseUrl}?isActive={!showDeleted}");
    }

    public async Task<Result<PaginatedList<SpeciesManagementModel>?>> GetSpeciesManagement(string searchQuery,
        int pageNumber,
        int pageSize,
        bool showDeleted,
        SortDefinition<SpeciesManagementModel>? sortDefinition = null,
        ICollection<IFilterDefinition<SpeciesManagementModel>>? filterDefinitions = null)
    {
        var query = $"searchQuery={searchQuery}";
        query += $"&pageNumber={pageNumber}";
        query += $"&pageSize={pageSize}";
        query += $"&isActive={!showDeleted}";

        if (sortDefinition != null)
            query += $"&sort={JsonSerializer.Serialize(sortDefinition.Adapt<Sort>())}";

        if (filterDefinitions != null && filterDefinitions.Count != 0)
        {
            var filters = filterDefinitions.Adapt<List<Filter>>();
            query += $"&filters={JsonSerializer.Serialize(filters)}";
        }
        
        var result = await _http.GetAsync<PaginatedList<SpeciesManagementModel>?>($"{_baseUrl}/management?{query}");

        return result;
    }

    public async Task<Result<SpeciesModel>> GetSpeciesById(int id)
    {
        return await _http.GetAsync<SpeciesModel>($"{_baseUrl}/{id}");
    }

    public async Task<Result<SpeciesModel>?> CreateSpecies(SpeciesModel species)
    {
        return await _http.PostAsync($"{_baseUrl}", species);
    }

    public async Task<Result<SpeciesModel>?> UpdateSpecies(SpeciesModel species)
    {
        return await _http.PutAsync($"{_baseUrl}/{species.Id}", species);
    }

    public async Task<Result<bool>> DeleteSpecies(int id)
    {
        return await _http.DeleteAsync<bool>($"{_baseUrl}/{id}");
    }

    public async Task<Result<bool>?> RestoreSpecies(int id)
    {
        return await _http.PutAsync($"{_baseUrl}/{id}/restore", false);
    }
}