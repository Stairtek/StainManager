using StainManager.Domain.Common;

namespace StainManager.Domain.Species;

public interface ISpeciesRepository
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

    Task<Species> UpdateSpeciesAsync(Species species);

    Task<bool> DeleteSpeciesAsync(int id);

    Task<bool> RestoreSpeciesAsync(int id);
}