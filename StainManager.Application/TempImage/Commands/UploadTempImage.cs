using StainManager.Application.Services;

namespace StainManager.Application.TempImage.Commands;

public class UploadTempImageCommand
    : ICommand<string>
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public required string ImageContent { get; set; }
}

public class UploadSpeciesImageCommandHandler(
    IImageService imageService)
    : ICommandHandler<UploadTempImageCommand, string>
{
    public async Task<Result<string>> Handle(
        UploadTempImageCommand request,
        CancellationToken cancellationToken)
    {
        var imageUploadResponse = await imageService.UploadImageAsync(
            "temp",
            request.Id,
            request.ImageContent);

        return imageUploadResponse.ETag;
    }
}