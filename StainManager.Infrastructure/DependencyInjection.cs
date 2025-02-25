using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
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

        var awsOptions = configuration.GetAWSOptions("AWS");
        var sharedCredentialsFile = new SharedCredentialsFile(awsOptions.ProfilesLocation);
        
        if (!sharedCredentialsFile.TryGetProfile(awsOptions.Profile, out var profile))
            throw new InvalidOperationException($"AWS profile '{awsOptions.Profile}' not found.");
        
        if (!AWSCredentialsFactory.TryGetAWSCredentials(profile, sharedCredentialsFile, out var awsCredentials))
            throw new InvalidOperationException($"AWS credentials for profile '{awsOptions.Profile}' not found.");

        var amazonS3Config = new AmazonS3Config
        {
            RegionEndpoint = awsOptions.Region
        };

        services.AddSingleton<IAmazonS3>(sp => new AmazonS3Client(awsCredentials, amazonS3Config));
        services.AddScoped<IImageService, ImageService>();

        return services;
    }
}