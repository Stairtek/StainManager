namespace StainManager.Application.Textures.Queries.GetTexturesForManagement;

public class GetTexturesForManagementQueryValidator
    : AbstractValidator<GetTexturesForManagementQuery>
{
    public GetTexturesForManagementQueryValidator()
    {
        RuleFor(c => c.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page Number must be at least greater than or equal to 1.");

        RuleFor(c => c.PageSize)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page Size must be at least greater than or equal to 1.");
    }
}