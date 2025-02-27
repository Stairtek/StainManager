using StainManager.Application.Textures.Commands.UpdateTexture;

namespace StainManager.Application.Species.Commands.UpdateSpecies;

public class UpdateTextureCommandValidator
    : AbstractValidator<UpdateTextureCommand>
{
    public UpdateTextureCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");
    }
}