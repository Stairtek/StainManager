using StainManager.Domain.Common;

namespace StainManager.Domain.Textures;

public interface ITextureRepository
{
    Task<List<Texture>> GetAllTexturesAsync(
        bool isActive = true);
    
    Task<PaginatedList<Texture>> GetTexturesForManagementAsync(
        string? searchQuery = "",
        int pageNumber = 1,
        int pageSize = 10,
        bool isActive = true,
        Sort? sort = null,
        List<Filter>? filters = null);
    
    Task<Texture?> GetTextureByIdAsync(
        int id,
        bool includeInactive = false);
    
    Task<Texture> CreateTextureAsync(Texture texture);
    
    Task<bool> UpdateTextureImageLocationsAsync(
        int id,
        string? fullImageLocation,
        string? thumbnailLocation);
    
    Task<Texture> UpdateTextureAsync(Texture texture);
    
    Task<bool> DeleteTextureAsync(int id);
    
    Task<bool> RestoreTextureAsync(int id);
}