using StainManager.Domain.Common;
using StainManager.Domain.Common.Interfaces;

namespace StainManager.Domain.Species;

public interface ISpeciesRepository : IRepository
{
    Task<List<Species>> GetAllSpeciesAsync(
        bool isActive = true);

    Task<PaginatedList<Species>> GetSpeciesForManagementAsync(
        string? searchQuery = "",
        int pageNumber = 1,
        int pageSize = 10,
        bool isActive = true,
        Sort? sort = null,
        List<Filter>? filters = null);

    Task<Species?> GetSpeciesByIdAsync(
        int id,
        bool includeInactive = false);

    Task<Species> CreateSpeciesAsync(Species species);
    
    Task<bool> UpdateSpeciesImageLocationsAsync(
        int id,
        string? imageLocation,
        string? thumbnailLocation);

    Task<Species> UpdateSpeciesAsync(Species species);

    Task<bool> DeleteSpeciesAsync(int id);

    Task<bool> RestoreSpeciesAsync(int id);
}