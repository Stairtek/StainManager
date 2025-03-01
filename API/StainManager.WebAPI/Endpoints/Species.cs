using StainManager.Application.Species.Commands.CreateSpecies;
using StainManager.Application.Species.Commands.DeleteSpecies;
using StainManager.Application.Species.Commands.RestoreSpecies;
using StainManager.Application.Species.Commands.UpdateSpecies;
using StainManager.Application.Species.Queries.GetSpecies;
using StainManager.Application.Species.Queries.GetSpeciesById;
using StainManager.Application.Species.Queries.GetSpeciesForManagement;

namespace StainManager.WebAPI.Endpoints;

public class Species : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetSpecies)
            .MapGet(GetSpeciesForManagement, "management")
            .MapGet(GetSpeciesById, "{id}")
            .MapPost(CreateSpecies)
            .MapPut(UpdateSpecies, "{id}")
            .MapPatch(RestoreSpecies, "{id}/restore")
            .MapDelete(DeleteSpecies, "{id}");
    }

    public async Task<IResult> GetSpecies(
        ISender sender,
        bool isActive = true)
    {
        var query = new GetSpeciesQuery { IsActive = isActive };
        var result = await sender.Send(query);

        return result.Failure
            ? Results.BadRequest(result)
            : Results.Ok(result);
    }

    public async Task<IResult> GetSpeciesForManagement(
        ISender sender,
        string searchQuery = "",
        int? pageNumber = null,
        int? pageSize = null,
        bool isActive = true,
        string? sort = null,
        string? filters = null)
    {
        var query = new GetSpeciesForManagementQuery
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

    public async Task<IResult> GetSpeciesById(
        ISender sender,
        int id)
    {
        var query = new GetSpeciesByIdQuery { Id = id };
        var result = await sender.Send(query);

        return result.Failure
            ? Results.BadRequest(result)
            : Results.Ok(result);
    }

    public async Task<IResult> CreateSpecies(
        ISender sender,
        CreateSpeciesCommand command)
    {
        var result = await sender.Send(command);

        return result.Failure
            ? Results.BadRequest(result)
            : Results.Created($"/{result.Value.Id}", result);
    }

    public async Task<IResult> UpdateSpecies(
        ISender sender,
        int id,
        UpdateSpeciesCommand command)
    {
        if (id != command.Id)
            return Results.BadRequest();

        var result = await sender.Send(command);

        return result.Failure
            ? Results.BadRequest(result)
            : Results.Ok(result);
    }

    public async Task<IResult> DeleteSpecies(
        ISender sender,
        int id)
    {
        var command = new DeleteSpeciesCommand { Id = id };
        var result = await sender.Send(command);

        return result.Failure
            ? Results.BadRequest(result)
            : Results.Ok(result);
    }

    public async Task<IResult> RestoreSpecies(
        ISender sender,
        int id)
    {
        var command = new RestoreSpeciesCommand { Id = id };
        var result = await sender.Send(command);

        return result.Failure
            ? Results.BadRequest(result)
            : Results.Ok(result);
    }
}