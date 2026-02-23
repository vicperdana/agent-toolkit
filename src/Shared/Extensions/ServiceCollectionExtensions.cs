using Microsoft.Extensions.DependencyInjection;

namespace Shared.Extensions;

/// <summary>
/// Extension methods for IServiceCollection to register shared services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds shared services to the dependency injection container.
    /// </summary>
    public static IServiceCollection AddSharedServices(this IServiceCollection services)
    {
        // Register shared services here as the application grows
        // Example: services.AddScoped<IMyService, MyService>();

        return services;
    }
}
