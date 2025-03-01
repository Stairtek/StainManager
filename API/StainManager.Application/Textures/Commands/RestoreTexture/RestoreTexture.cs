using StainManager.Domain.Common;
using StainManager.Domain.Textures;

namespace StainManager.Application.Textures.Commands.RestoreTexture;

public class RestoreTextureCommand
    : ICommand
{
    public int Id { get; set; }
}


public class RestoreTextureCommandHandler(
    ITextureRepository textureRepository)
    : ICommandHandler<RestoreTextureCommand>
{
    public async Task<Result> Handle(
        RestoreTextureCommand request,
        CancellationToken cancellationToken)
    {
        var result = await textureRepository
            .RestoreTextureAsync(request.Id);

        return !result
            ? Result.Fail<object>("Failed to restore texture", true)
            : Result.Ok();
    }
}