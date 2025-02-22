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
    
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<SpeciesModel>
            .CreateWithOptions(
                (SpeciesModel)model, 
                x => x.IncludeProperties(propertyName)));
        
        if (result.IsValid)
            return Array.Empty<string>();
        
        return result.Errors.Select(e => e.ErrorMessage);
    };
}