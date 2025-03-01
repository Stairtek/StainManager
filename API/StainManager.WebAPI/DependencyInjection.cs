namespace StainManager.WebAPI;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services)
    {
        services.AddExceptionHandler<CustomExceptionHandler>();

        services.AddOpenApi();
        services.AddControllers();
        services.AddAntiforgery();
        services.AddHttpContextAccessor();

        return services;
    }
}