namespace StainManager.Application.Species.Queries.GetSpeciesForManagement;

public class GetSpeciesForManagementQueryValidator
    : AbstractValidator<GetSpeciesForManagementQuery>
{
    public GetSpeciesForManagementQueryValidator()
    {
        RuleFor(c => c.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page Number must be at least greater than or equal to 1.");

        RuleFor(c => c.PageSize)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page Size must be at least greater than or equal to 1.");
    }
}