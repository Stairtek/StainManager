using StainManager.Application.Textures.Commands.CreateTexture;
using StainManager.Application.Textures.Commands.DeleteTexture;
using StainManager.Application.Textures.Commands.RestoreTexture;
using StainManager.Application.Textures.Commands.UpdateTexture;
using StainManager.Application.Textures.Commands.UpdateTexturesSortOrder;
using StainManager.Application.Textures.Queries.GetTextureById;
using StainManager.Application.Textures.Queries.GetTextures;
using StainManager.Application.Textures.Queries.GetTexturesForManagement;
using StainManager.Application.Textures.Queries.GetTexturesSummary;

namespace StainManager.WebAPI.Endpoints;

public class Textures
    : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetTextures)
            .MapGet(GetTexturesSummary, "summary")
            .MapGet(GetTexturesForManagement, "management")
            .MapGet(GetTextureById, "{id}")
            .MapPost(CreateTexture)
            .MapPut(UpdateTexture, "{id}")
            .MapPut(RestoreTexture, "{id}/restore")
            .MapPatch(UpdateTexturesSortOrder, "sortorder")
            .MapDelete(DeleteTexture, "{id}");
    }
    
    public async Task<IResult> GetTextures(
        ISender sender,
        bool isActive = true)
    {
        var query = new GetTexturesQuery { IsActive = isActive };
        var result = await sender.Send(query);

        return result.Failure
            ? Results.BadRequest(result)
            : Results.Ok(result);
    }

    public async Task<IResult> GetTexturesSummary(
        ISender sender)
    {
        var query = new GetTexturesSummaryQuery();
        var result = await sender.Send(query);
        
        return result.Failure
            ? Results.BadRequest(result)
            : Results.Ok(result);
    }

    public async Task<IResult> GetTexturesForManagement(
        ISender sender,
        string searchQuery = "",
        int? pageNumber = null,
        int? pageSize = null,
        bool isActive = true,
        string? sort = null,
        string? filters = null)
    {
        var query = new GetTexturesForManagementQuery
        {
            SearchQuery = searchQuery,
            PageNumber = pageNumber is null or < 1 ? 1 : pageNumber.Value,
            PageSize = pageSize ?? 10,
            IsActive = isActive
        };
        
        if (sort is not null)
            query.Sort = JsonSerializer.Deserialize<Sort>(sort);
        
        if (filters is not null)
            query.Filters = JsonSerializer.Deserialize<List<Filter>>(filters);
        
        var result = await sender.Send(query);
        
        return result.Failure
            ? Results.BadRequest(result)
            : Results.Ok(result);
    }
    
    public async Task<IResult> GetTextureById(
        ISender sender,
        int id)
    {
        var query = new GetTextureByIdQuery { Id = id };
        var result = await sender.Send(query);

        return result.Failure
            ? Results.BadRequest(result)
            : Results.Ok(result);
    }
    
    public async Task<IResult> CreateTexture(
        ISender sender,
        CreateTextureCommand command)
    {
        var result = await sender.Send(command);

        return result.Failure
            ? Results.BadRequest(result)
            : Results.Created($"/{result.Value.Id}", result);
    }
    
    public async Task<IResult> UpdateTexture(
        ISender sender,
        int id,
        UpdateTextureCommand command)
    {
        if (id != command.Id)
            return Results.BadRequest();
        
        var result = await sender.Send(command);

        return result.Failure
            ? Results.BadRequest(result)
            : Results.Ok(result);
    }
    
    public async Task<IResult> UpdateTexturesSortOrder(
        ISender sender,
        List<SortOrderModel> textures)
    {
        var command = new UpdateTexturesSortOrderCommand
        {
            Textures = textures
        };
        var result = await sender.Send(command);

        return result.Failure
            ? Results.BadRequest(result)
            : Results.Ok(result);
    }
    
    public async Task<IResult> DeleteTexture(
        ISender sender,
        int id)
    {
        var command = new DeleteTextureCommand { Id = id };
        var result = await sender.Send(command);

        return result.Failure
            ? Results.BadRequest(result)
            : Results.Ok(result);
    }
    
    public async Task<IResult> RestoreTexture(
        ISender sender,
        int id)
    {
        var command = new RestoreTextureCommand { Id = id };
        var result = await sender.Send(command);

        return result.Failure
            ? Results.BadRequest(result)
            : Results.Ok(result);
    }
}