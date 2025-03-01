using StainManager.Domain.Common;
using StainManager.Domain.Textures;

namespace StainManager.Application.Textures.Commands.DeleteTexture;

public class DeleteTextureCommand
    : ICommand
{
    public int Id { get; set; }
}


public class DeleteTextureCommandHandler(
    ITextureRepository textureRepository)
    : ICommandHandler<DeleteTextureCommand>
{
    public async Task<Result> Handle(
        DeleteTextureCommand request,
        CancellationToken cancellationToken)
    {
        var result = await textureRepository
            .DeleteTextureAsync(request.Id);

        return result
            ? Result.Ok()
            : Result.Fail<object>("Failed to delete texture", true);
    }
}