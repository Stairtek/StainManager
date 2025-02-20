using StainManager.Domain.Species;
using StainManager.Infrastructure.Repositories;

namespace StainManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddScoped<ISpeciesRepository, SpeciesRepository>();

        return services;
    }
}