using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using StainManager.Application.TempImage.Commands;

namespace StainManager.WebAPI.Endpoints;

public class TempImage : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(UploadTempImage);
    }

    [ValidateAntiForgeryToken]
    public async Task<IResult> UploadTempImage(
        ISender sender,
        [FromBody] UploadTempImageCommand command)
    {
        if (string.IsNullOrEmpty(command.ImageContent))
            return Results.BadRequest("Image content is required");
        
        var result = await sender.Send(command);

        return result.Failure
            ? Results.BadRequest(result)
            : Results.Ok(result);
    }
}