namespace StainManager.Domain.Species;

public interface ISpeciesRepository
{
    Task<List<Species>> GetAllSpeciesAsync(
        bool isActive = true);
    
    Task<Species?> GetSpeciesByIdAsync(
        int id, 
        bool includeInactive = false);
    
    Task<Species> CreateSpeciesAsync(Species species);
    
    Task<Species?> UpdateSpeciesAsync(Species species);
    
    Task<bool> DeleteSpeciesAsync(int id);
    
    Task<bool> RestoreSpeciesAsync(int id);
}