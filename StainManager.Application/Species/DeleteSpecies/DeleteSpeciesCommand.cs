using StainManager.Application.Common.RequestHandling;

namespace StainManager.Application.Species.DeleteSpecies;

public class DeleteSpeciesCommand
    : ICommand
{
    public int Id { get; set; }
}