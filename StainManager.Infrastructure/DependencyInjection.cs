using Amazon.S3;
using StainManager.Application.Services;
using StainManager.Domain.Species;
using StainManager.Infrastructure.Repositories;
using StainManager.Infrastructure.Services.S3;

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

        services.AddSingleton<IAmazonS3, AmazonS3Client>();
        services.AddScoped<IImageService, ImageService>();

        return services;
    }
}