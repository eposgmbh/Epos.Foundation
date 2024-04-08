using System;

using Microsoft.AspNetCore.Builder;

using Epos.Utilities.Web;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary> Provides extensions methods for the <see cref="ObjectSession" />
/// class. </summary>
public static class ObjectSessionServiceCollectionExtensions
{
    /// <summary> Configures and adds an object session to the service collection.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddObjectSession(this IServiceCollection services) =>
        services.AddObjectSession(configure: null!);

    /// <summary> Configures and adds an object session to the service collection.
    /// </summary>
    /// <param name="services">Service Collection</param>
    /// <param name="configure">Configuration delegate</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddObjectSession(
        this IServiceCollection services, Action<SessionOptions> configure
    ) {
        services.AddDistributedMemoryCache();

        if (configure is not null) {
            services.AddSession(configure);
        } else {
            services.AddSession();
        }

        services.AddHttpContextAccessor();
        services.AddScoped<IObjectSession, ObjectSession>();

        return services;
    }
}
