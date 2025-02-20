namespace StainManager.Application.Species.GetSpeciesById;

public class GetSpeciesByIdQuery
    : IQuery<SpeciesResponse>
{
    public int Id { get; set; }
}