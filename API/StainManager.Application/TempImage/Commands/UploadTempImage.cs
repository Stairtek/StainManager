using StainManager.Application.Services;
using StainManager.Domain.Common;

namespace StainManager.Application.TempImage.Commands;

public class UploadTempImageCommand
    : ICommand<ImageUploadResult>
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public required string ImageContent { get; set; }
    
    public string? FileName { get; set; }
    
    public string? MediaType { get; set; }
}

public class UploadSpeciesImageCommandHandler(
    IImageService imageService)
    : ICommandHandler<UploadTempImageCommand, ImageUploadResult>
{
    public async Task<Result<ImageUploadResult>> Handle(
        UploadTempImageCommand request,
        CancellationToken cancellationToken)
    {
        var imageURL = await imageService.UploadImageAsync(
            "temp",
            request.Id,
            request.ImageContent,
            request.FileName,
            request.MediaType);

        return imageURL;
    }
}