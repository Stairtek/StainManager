using StainManager.Domain.Common;
using StainManager.Domain.Textures;

namespace StainManager.Application.Textures.Commands.UpdateTexturesSortOrder;

public class UpdateTexturesSortOrderCommand
    : ICommand
{
    public List<SortOrderModel> Textures { get; set; } = [];
}


public class UpdateTexturesSortOrderCommandHandler(
    ITextureRepository textureRepository)
    : ICommandHandler<UpdateTexturesSortOrderCommand>
{
    public async Task<Result> Handle(
        UpdateTexturesSortOrderCommand request,
        CancellationToken cancellationToken)
    {
        var result = await textureRepository
            .UpdateTexturesSortOrderAsync(request.Textures);

        return result
            ? Result.Ok()
            : Result.Fail<object>("Failed to update textures sort order", true);
    }
}