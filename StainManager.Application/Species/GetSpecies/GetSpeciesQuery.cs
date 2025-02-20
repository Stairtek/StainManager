namespace StainManager.Application.Species.GetSpecies;

public class GetSpeciesQuery
    : IQuery<List<SpeciesResponse>>
{
    public bool IsActive { get; set; } = true;
}