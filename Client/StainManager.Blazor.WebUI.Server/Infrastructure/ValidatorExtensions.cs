using FluentValidation;

namespace StainManager.Blazor.WebUI.Server.Infrastructure;

public static class ValidatorExtensions
{
    public static Func<object, string, Task<IEnumerable<string>>> ValidateValue<T>(
        this AbstractValidator<T> validator) 
        where T : class
    {
        return async (model, propertyName) =>
        {
            var result = await validator.ValidateAsync(ValidationContext<T>
                .CreateWithOptions(
                    (T)model,
                    x => x.IncludeProperties(propertyName)));

            if (result.IsValid)
                return Array.Empty<string>();

            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}