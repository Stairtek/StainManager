using StainManager.Domain.Common;
using StainManager.Domain.Textures;

namespace StainManager.Application.Textures.Queries.GetTextureById;

public class GetTextureByIdQuery
    : IQuery<TextureResponse>
{
    public int Id { get; set; }
}


public class GetTextureByIdQueryHandler(
    ITextureRepository textureRepository)
    : IQueryHandler<GetTextureByIdQuery, TextureResponse>
{
    public async Task<Result<TextureResponse>> Handle(
        GetTextureByIdQuery request,
        CancellationToken cancellationToken)
    {
        var texture = await textureRepository
            .GetTextureByIdAsync(request.Id);
        
        if (texture is null)
            return Result.Fail<TextureResponse>($"Texture with ID {request.Id} not found.", true);
        
        var response = texture.Adapt<TextureResponse>();

        return response;
    }
}