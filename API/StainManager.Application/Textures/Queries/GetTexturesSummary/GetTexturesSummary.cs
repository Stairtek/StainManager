using StainManager.Domain.Common;
using StainManager.Domain.Textures;

namespace StainManager.Application.Textures.Queries.GetTexturesSummary;

public class GetTexturesSummaryQuery
    : IQuery<List<TextureSummaryResponse>>
{
}


public class GetTexturesSummaryQueryHandler(
    ITextureRepository textureRepository)
    : IQueryHandler<GetTexturesSummaryQuery, List<TextureSummaryResponse>>
{
    public async Task<Result<List<TextureSummaryResponse>>> Handle(
        GetTexturesSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var textures = await textureRepository
            .GetAllTexturesAsync();
        var response = textures.Adapt<List<TextureSummaryResponse>>();

        return response;
    }
}