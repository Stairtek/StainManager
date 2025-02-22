using System.Net;
using StainManager.Blazor.WebUI.Server.Features.Species.Models;

namespace StainManager.Blazor.WebUI.Server.Features.Species.Services;

public interface ISpeciesService
{
    Task<List<SpeciesModel>?> GetSpecies(bool showDeleted = false);
    
    Task<SpeciesModel?> GetSpeciesById(int id);
    
    Task<HttpResponseMessage> CreateSpecies(SpeciesModel species);
    
    Task<HttpResponseMessage> UpdateSpecies(SpeciesModel species);
    
    Task<HttpResponseMessage> DeleteSpecies(int id);
    
    Task<HttpResponseMessage> RestoreSpecies(int id);
}

public class SpeciesService(
    HttpClient http) 
    : ISpeciesService
{
    private readonly string _baseUrl = "species";
    
    public async Task<List<SpeciesModel>?> GetSpecies(bool showDeleted = false)
    {
        return await http.GetFromJsonAsync<List<SpeciesModel>>($"{_baseUrl}?isActive={!showDeleted}");
    }

    public async Task<SpeciesModel?> GetSpeciesById(int id)
    {
        return await http.GetFromJsonAsync<SpeciesModel?>($"{_baseUrl}/{id}");
    }
    
    public async Task<HttpResponseMessage> CreateSpecies(SpeciesModel species)
    {
        return await http.PostAsJsonAsync($"{_baseUrl}", species);
    }
    
    public async Task<HttpResponseMessage> UpdateSpecies(SpeciesModel species)
    {
        return await http.PutAsJsonAsync($"{_baseUrl}", species);
    }

    public async Task<HttpResponseMessage> DeleteSpecies(int id)
    {
        return await http.DeleteAsync($"{_baseUrl}/{id}");
    }

    public async Task<HttpResponseMessage> RestoreSpecies(int id)
    {
        return await http.PutAsync($"{_baseUrl}/{id}/restore", null);
    }
}