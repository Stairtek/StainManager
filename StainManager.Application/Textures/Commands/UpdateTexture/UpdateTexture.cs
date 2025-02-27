using StainManager.Application.Services;
using StainManager.Domain.Common;
using StainManager.Domain.Textures;

namespace StainManager.Application.Textures.Commands.UpdateTexture;

public class UpdateTextureCommand
    : ICommand<TextureResponse?>
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string? FullImageLocation { get; set; }
    
    public string? ThumbnailImageLocation { get; set; }
}


public class UpdateTextureCommandHandler(
    ITextureRepository textureRepository,
    IImageService imageService)
    : ICommandHandler<UpdateTextureCommand, TextureResponse?>
{
    public async Task<Result<TextureResponse?>> Handle(
        UpdateTextureCommand request,
        CancellationToken cancellationToken)
    {
        var updatedTexture = request.Adapt<Texture>();

        var hasTempImages =
            updatedTexture.FullImageLocation?.Contains("temp") == true ||
            updatedTexture.ThumbnailImageLocation?.Contains("temp") == true;

        if (hasTempImages)
        {
            var moveImagesResult = await imageService.MoveImagesAsync(
                updatedTexture.FullImageLocation,
                updatedTexture.ThumbnailImageLocation,
                "textures",
                updatedTexture.Id);

            if (moveImagesResult.Failure)
                return Result.Fail<TextureResponse?>(moveImagesResult.Error, moveImagesResult.HandledError);

            updatedTexture.FullImageLocation = moveImagesResult.Value?.FullImageLocation;
            updatedTexture.ThumbnailImageLocation = moveImagesResult.Value?.ThumbnailImageLocation;
        }

        var result = await textureRepository.UpdateTextureAsync(updatedTexture);

        return request.Adapt<TextureResponse>();
    }
}