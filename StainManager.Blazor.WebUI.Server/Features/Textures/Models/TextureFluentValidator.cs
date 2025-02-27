using StainManager.Blazor.WebUI.Server.Extensions;
using FluentValidation;

namespace StainManager.Blazor.WebUI.Server.Features.Textures.Models;

public class TextureFluentValidator
    : AbstractValidator<TextureModel>
{
    public TextureFluentValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must be less than 200 characters.");
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue 
        => this.ValidateValue<TextureModel>();
}