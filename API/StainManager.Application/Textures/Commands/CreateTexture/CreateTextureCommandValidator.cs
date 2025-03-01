namespace StainManager.Application.Textures.Commands.CreateTexture;

public class CreateTextureCommandValidator
    : AbstractValidator<CreateTextureCommand>
{
    public CreateTextureCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");
    }
}