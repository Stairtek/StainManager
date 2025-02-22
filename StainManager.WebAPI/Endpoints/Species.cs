using System.Text.Json;
using StainManager.Application.Species.Commands.CreateSpecies;
using StainManager.Application.Species.Commands.DeleteSpecies;
using StainManager.Application.Species.Commands.RestoreSpecies;
using StainManager.Application.Species.Commands.UpdateSpecies;
using StainManager.Application.Species.Queries.GetSpecies;
using StainManager.Application.Species.Queries.GetSpeciesById;
using StainManager.Application.Species.Queries.GetSpeciesForManagement;
using StainManager.Domain.Common;

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
            .MapPut(RestoreSpecies, "{id}/restore")
            .MapDelete(DeleteSpecies, "{id}");
    }

    public async Task<IResult> GetSpecies(
        ISender sender,
        bool isActive = true)
    {
        var query = new GetSpeciesQuery { IsActive = isActive };
        var result = await sender.Send(query);

        return result.Failure
            ? Results.BadRequest(result.Error)
            : Results.Ok(result.Value);
    }

    public async Task<IResult> GetSpeciesForManagement(
        ISender sender,
        string searchQuery = "",
        bool isActive = true,
        int? pageNumber = null,
        int? pageSize = null,
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
            ? Results.BadRequest(result.Error)
            : Results.Ok(result.Value);
    }

    public async Task<IResult> GetSpeciesById(
        ISender sender,
        int id)
    {
        var query = new GetSpeciesByIdQuery { Id = id };
        var result = await sender.Send(query);

        return result.Failure
            ? Results.BadRequest(result.Error)
            : Results.Ok(result.Value);
    }

    public async Task<IResult> CreateSpecies(
        ISender sender,
        CreateSpeciesCommand createSpeciesCommand)
    {
        var createResult = await sender.Send(createSpeciesCommand);

        return createResult.Failure
            ? Results.BadRequest(createResult.Error)
            : Results.Created($"/{createResult.Value.Id}", createResult.Value);
    }

    public async Task<IResult> UpdateSpecies(
        ISender sender,
        int id,
        UpdateSpeciesCommand updateSpeciesCommand)
    {
        if (id != updateSpeciesCommand.Id)
            return Results.BadRequest();

        var updateResult = await sender.Send(updateSpeciesCommand);

        return updateResult.Failure
            ? Results.BadRequest(updateResult.Error)
            : Results.Ok(updateResult.Value);
    }

    public async Task<IResult> DeleteSpecies(
        ISender sender,
        int id)
    {
        var deleteResult = await sender.Send(new DeleteSpeciesCommand { Id = id });

        return deleteResult.Failure
            ? Results.BadRequest(deleteResult.Error)
            : Results.Ok();
    }

    public async Task<IResult> RestoreSpecies(
        ISender sender,
        int id)
    {
        var restoreResult = await sender.Send(new RestoreSpeciesCommand { Id = id });

        return restoreResult.Failure
            ? Results.BadRequest(restoreResult.Error)
            : Results.Ok();
    }
}