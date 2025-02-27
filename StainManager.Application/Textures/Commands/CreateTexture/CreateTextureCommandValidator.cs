namespace StainManager.Application.Textures.Commands.CreateTexture;

public class CreateTextureCommandValidator
    : AbstractValidator<CreateTextureCommand>
{
    public CreateTextureCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");
    }
}