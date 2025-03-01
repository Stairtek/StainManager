using StainManager.Domain.Common;
using StainManager.Domain.Textures;

namespace StainManager.Application.Textures.Queries.GetTexturesForManagement;

public class GetTexturesForManagementQuery
    : IQuery<PaginatedList<TextureManagementResponse>>
{
    public string? SearchQuery { get; set; }
    
    public int PageNumber { get; set; } = 1;
    
    public int PageSize { get; set; } = 10;
    
    public bool IsActive { get; set; } = true;
    
    public Sort? Sort { get; set; }
    
    public List<Filter>? Filters { get; set; }
}


public class GetTexturesForManagementQueryHandler(
    ITextureRepository textureRepository)
    : IQueryHandler<GetTexturesForManagementQuery, PaginatedList<TextureManagementResponse>>
{
    public async Task<Result<PaginatedList<TextureManagementResponse>>> Handle(
        GetTexturesForManagementQuery request,
        CancellationToken cancellationToken)
    {
        var textures = await textureRepository
            .GetTexturesForManagementAsync(
                request.SearchQuery,
                request.PageNumber,
                request.PageSize,
                request.IsActive,
                request.Sort,
                request.Filters);
        
        var constructor = typeof(PaginatedList<TextureManagementResponse>)
            .GetConstructor([typeof(List<TextureManagementResponse>), typeof(int), typeof(int), typeof(int)]);
        
        if (constructor is null)
            throw new InvalidOperationException("Constructor not found");
        
        TypeAdapterConfig<PaginatedList<Texture>, PaginatedList<TextureManagementResponse>>
            .NewConfig()
            .MapToConstructor(constructor);
        var response = textures.Adapt<PaginatedList<TextureManagementResponse>>();
        
        return response;
    }
}