using StainManager.Application.Species.Commands.CreateSpecies;
using StainManager.Application.Species.Commands.DeleteSpecies;
using StainManager.Application.Species.Commands.RestoreSpecies;
using StainManager.Application.Species.Commands.UpdateSpecies;
using StainManager.Application.Species.Queries.GetSpecies;
using StainManager.Application.Species.Queries.GetSpeciesById;

namespace StainManager.WebAPI.Endpoints;

public static class SpeciesEndpoints
{
    public static RouteGroupBuilder MapSpeciesEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", 
            async (ISender sender, bool isActive = true) =>
            {
                var query = new GetSpeciesQuery { IsActive = isActive };
                var result = await sender.Send(query);
                
                return result.Failure 
                    ? Results.BadRequest(result.Error) 
                    : Results.Ok(result.Value);
            });
        
        group.MapGet("/{id:int}",
            async (ISender sender,
                int id) =>
            {
                var query = new GetSpeciesByIdQuery { Id = id };
                var result = await sender.Send(query);
                
                return result.Failure 
                    ? Results.BadRequest(result.Error) 
                    : Results.Ok(result.Value);
            });

        group.MapPost("/",
            async (ISender sender,
                CreateSpeciesCommand createSpeciesCommand) =>
            {
                var createResult = await sender.Send(createSpeciesCommand);
                
                return createResult.Failure 
                    ? Results.BadRequest(createResult.Error) 
                    : Results.Created($"/{createResult.Value.Id}", createResult.Value);
            });
        
        group.MapPut("/",
            async (ISender sender,
                UpdateSpeciesCommand updateSpeciesCommand) =>
            {
                var updateResult = await sender.Send(updateSpeciesCommand);
                
                return updateResult.Failure 
                    ? Results.BadRequest(updateResult.Error) 
                    : Results.Ok(updateResult.Value);
            });
        
        group.MapDelete("/{id:int}", async (ISender sender, int id) =>
        {
            var deleteResult = await sender.Send(new DeleteSpeciesCommand { Id = id });
            
            return deleteResult.Failure 
                ? Results.BadRequest(deleteResult.Error) 
                : Results.Ok();
        });
        
        group.MapPut("/{id:int}/restore", async (ISender sender, int id) =>
        {
            var restoreResult = await sender.Send(new RestoreSpeciesCommand { Id = id });
            
            return restoreResult.Failure 
                ? Results.BadRequest(restoreResult.Error) 
                : Results.Ok();
        });

        return group;
    }
}