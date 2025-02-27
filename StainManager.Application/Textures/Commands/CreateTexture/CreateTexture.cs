using StainManager.Application.Services;
using StainManager.Domain.Common;
using StainManager.Domain.Textures;

namespace StainManager.Application.Textures.Commands.CreateTexture;

public class CreateTextureCommand
    : ICommand<TextureResponse>
{
    public required string Name { get; set; }
    
    public string? FullImageLocation { get; set; }

    public string? ThumbnailImageLocation { get; set; }
}


public class CreateTextureCommandHandler(
    ITextureRepository textureRepository,
    IImageService imageService)
    : ICommandHandler<CreateTextureCommand, TextureResponse>
{
    public async Task<Result<TextureResponse>> Handle(
        CreateTextureCommand request,
        CancellationToken cancellationToken)
    {
        var newTexture = request.Adapt<Texture>();
        var texture = await textureRepository.CreateTextureAsync(newTexture);
        
        if (texture.FullImageLocation is null || texture.ThumbnailImageLocation is null) 
            return texture.Adapt<TextureResponse>();
        
        var moveImagesResult = await imageService.MoveImagesAsync(
            texture.FullImageLocation,
            texture.ThumbnailImageLocation,
            "textures",
            texture.Id);
        
        if (moveImagesResult.Failure)
            return Result.Fail<TextureResponse>(moveImagesResult.Error, moveImagesResult.HandledError);
        
        texture.FullImageLocation = moveImagesResult.Value?.FullImageLocation;
        texture.ThumbnailImageLocation = moveImagesResult.Value?.ThumbnailImageLocation;
        
        var updateTextureResult = await textureRepository.UpdateTextureImageLocationsAsync(
            texture.Id,
            texture.FullImageLocation,
            texture.ThumbnailImageLocation);
        
        if (updateTextureResult is false)
            return Result.Fail<TextureResponse>("Failed to update texture image locations", true);

        return texture.Adapt<TextureResponse>();
    }
}