namespace StainManager.Application.Species.Commands.UpdateSpecies;

public class UpdateSpeciesCommandValidator
    : AbstractValidator<UpdateSpeciesCommand>
{
    public UpdateSpeciesCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(c => c.Abbreviation)
            .Length(3)
            .When(c => c.IsProduction)
            .WithMessage("Abbreviation must be 3 characters.");
    }
}