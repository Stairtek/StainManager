using StainManager.Application.Common.RequestHandling;

namespace StainManager.Application.Species.RestoreSpecies;

public class RestoreSpeciesCommand
    : ICommand
{
    public int Id { get; set; }
}