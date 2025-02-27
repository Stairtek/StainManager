using StainManager.Domain.Common;
using StainManager.Domain.Textures;

namespace StainManager.Application.Textures.Queries.GetTextures;

public class GetTexturesQuery
    : IQuery<List<TextureResponse>>
{
    public bool IsActive { get; set; } = true;
}


public class GetTexturesQueryHandler(
    ITextureRepository textureRepository)
    : IQueryHandler<GetTexturesQuery, List<TextureResponse>>
{
    public async Task<Result<List<TextureResponse>>> Handle(
        GetTexturesQuery request,
        CancellationToken cancellationToken)
    {
        var textures = await textureRepository
            .GetAllTexturesAsync(request.IsActive);
        var response = textures.Adapt<List<TextureResponse>>();

        return response;
    }
}