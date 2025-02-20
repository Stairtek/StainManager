using StainManager.Domain.Species;

namespace StainManager.Infrastructure.Repositories;

public class SpeciesRepository(
    ApplicationDbContext context)
    : ISpeciesRepository
{
    public async Task<List<Species>> GetAllSpeciesAsync(bool isActive = true)
    {
        var result = await context.Species
            .Where(c => c.IsActive == isActive)
            .ToListAsync();

        return result;
    }

    public async Task<Species?> GetSpeciesByIdAsync(int id, bool includeInactive = false)
    {
        var speciesQuery = context.Species
            .Where(c => c.Id == id);

        if (!includeInactive)
            speciesQuery = speciesQuery.Where(c => c.IsActive);
        
        var species = await speciesQuery
            .FirstOrDefaultAsync();
        
        return species;
    }

    public async Task<Species> CreateSpeciesAsync(Species species)
    {
        context.Species.Add(species);
        
        await context.SaveChangesAsync();
        
        return species;
    }

    public async Task<Species?> UpdateSpeciesAsync(Species species)
    {
        var speciesToUpdate = await GetSpeciesByIdAsync(species.Id);
        
        if (speciesToUpdate == null)
            return null;
        
        speciesToUpdate.UpdatedBy = "System";
        speciesToUpdate.UpdatedDateTime = DateTime.Now;
        speciesToUpdate.Name = species.Name;
        speciesToUpdate.Abbreviation = species.Abbreviation;
        speciesToUpdate.IsProduction = species.IsProduction;
        speciesToUpdate.FullImageLocation = species.FullImageLocation;
        speciesToUpdate.ThumbnailImageLocation = species.ThumbnailImageLocation;
        speciesToUpdate.ScientificName = species.ScientificName;
        speciesToUpdate.CountryOfOrigin = species.CountryOfOrigin;
        speciesToUpdate.JankaHardness = species.JankaHardness;
        
        await context.SaveChangesAsync();
        
        return speciesToUpdate;
    }

    public async Task<bool> DeleteSpeciesAsync(int id)
    {
        var speciesToDelete = await GetSpeciesByIdAsync(id);
        
        if (speciesToDelete == null)
            return false;
        
        speciesToDelete.IsActive = false;
        speciesToDelete.UpdatedBy = "System";
        speciesToDelete.UpdatedDateTime = DateTime.Now;
        
        await context.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> RestoreSpeciesAsync(int id)
    {
        var speciesToRestore = await GetSpeciesByIdAsync(id, true);
        
        if (speciesToRestore == null)
            return false;
        
        speciesToRestore.IsActive = true;
        speciesToRestore.UpdatedBy = "System";
        speciesToRestore.UpdatedDateTime = DateTime.Now;
        
        await context.SaveChangesAsync();
        
        return true;
    }
}