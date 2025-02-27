using System.Reflection;
using StainManager.Application.Common.Behaviors;
using StainManager.Application.Common.Helpers;

namespace StainManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            configuration.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        });
        
        services.AddScoped<ICodeGenerator, CodeGenerator>();

        return services;
    }
}