using StainManager.Application.Species.CreateSpecies;
using StainManager.Application.Species.GetSpecies;
using StainManager.Application.Species.GetSpeciesById;

namespace StainManager.WebAPI.Endpoints;

public static class SpeciesEndpoints
{
    public static RouteGroupBuilder MapSpeciesEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (ISender sender) 
            => await sender.Send(new GetSpeciesQuery()));
        
        group.MapGet("/{id:int}", async (ISender sender, int id) 
            => await sender.Send(new GetSpeciesByIdQuery { Id = id }));

        group.MapPost("/",
            async (ISender sender,
                CreateSpeciesCommand createSpeciesCommand) =>
            {
                var createResult = await sender.Send(createSpeciesCommand);
                
                return createResult.Failure 
                    ? Results.BadRequest(createResult.Error) 
                    : Results.Created($"/{createResult.Value.Id}", createResult.Value);
            });

        return group;
    }
}