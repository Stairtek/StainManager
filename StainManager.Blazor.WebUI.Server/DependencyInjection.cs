using StainManager.Blazor.WebUI.Server.Common.Interfaces;

namespace StainManager.Blazor.WebUI.Server;

public static class DependencyInjection
{
    public static IServiceCollection AddWebAPIServices(
        this IServiceCollection services)
    {
        var serviceType = typeof(IWebAPIService).Assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .Where(t => t.GetInterfaces().Any(i => i.Name.EndsWith("Service")));
        
        foreach (var type in serviceType)
        {
            var interfaceType = type.GetInterfaces()
                .FirstOrDefault(i => i.Name.EndsWith("Service"));
            
            if (interfaceType != null)
                services.AddScoped(interfaceType, type);
        }

        return services;
    }
}