using System.Reflection;
using Amazon;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using StainManager.Application.Services;
using StainManager.Domain.Common.Interfaces;
using StainManager.Domain.Species;
using StainManager.Domain.Textures;
using StainManager.Infrastructure.Repositories;
using StainManager.Infrastructure.Services.S3;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace StainManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        ILogger logger)
    {
        

        services.AddRepositories();
        
        services.AddAWS(configuration);
        
        services.AddScoped<IImageService, ImageService>();

        return services;
    }

    private static IServiceCollection AddRepositories(
        this IServiceCollection services)
    {
        var repositoryInterfaceType = typeof(IRepository);
        var repositoryTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .Where(t => t.GetInterfaces().Contains(repositoryInterfaceType));

        foreach (var repositoryType in repositoryTypes)
        {
            var interfaceType = repositoryType.GetInterfaces()
                .First(i => i != repositoryInterfaceType);
            
            services.AddScoped(interfaceType, repositoryType);
        }

        return services;
    }

    private static IServiceCollection AddAWS(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        try
        {
            var awsOptions = configuration.GetAWSOptions("AWS");
            
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Local")
            {
                services.AddSingleton<IAmazonS3>(sp => new AmazonS3Client(awsOptions.Region));
                return services;
            }
            
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
        }
        catch (Exception ex)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Local")
                throw new InvalidOperationException("Failed to configure AWS services.", ex);
            
            services.AddSingleton<IAmazonS3>(sp => new AmazonS3Client(RegionEndpoint.USEast2));
            return services;

        }

        return services;
    }
}
