using StainManager.Blazor.WebUI.Server.Extensions;
using FluentValidation;

namespace StainManager.Blazor.WebUI.Server.Features.Species.Models;

public class SpeciesFluentValidator : AbstractValidator<SpeciesModel>
{
    public SpeciesFluentValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(c => c.Abbreviation)
            .NotEmpty()
                .When(c => c.IsProduction)
                .WithMessage("Abbreviation is required.")
            .Length(3)
                .When(c => c.IsProduction)
                .WithMessage("Abbreviation must be 3 characters.");
    }
    
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue 
        => this.ValidateValue<SpeciesModel>();
}