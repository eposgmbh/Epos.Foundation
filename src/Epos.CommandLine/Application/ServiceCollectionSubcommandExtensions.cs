using System;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Epos.CommandLine.Application;

/// <summary> Extension methods for the <see cref="ServiceCollection"/> class.
/// </summary>
public static class ServiceCollectionSubcommandExtensions
{
    /// <summary> Registers a subcommand. </summary>
    /// <typeparam name="TSubcommand">Subcommand type</typeparam>
    /// <param name="serviceCollection"><see cref="IServiceCollection"/> instance</param>
    /// <returns></returns>
    public static IServiceCollection AddSubcommand<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TSubcommand
    >(this IServiceCollection serviceCollection)
        where TSubcommand : class, ISubcommand {
        ArgumentNullException.ThrowIfNull(serviceCollection);

        return serviceCollection.AddSingleton<ISubcommand, TSubcommand>();
    }

    /// <summary> Configures the command line definition. </summary>
    /// <param name="hostApplicationBuilder"><see cref="HostApplicationBuilder"/> instance</param>
    /// <param name="configure">Configure delegate</param>
    /// <returns><see cref="HostApplicationBuilder"/> instance</returns>
    public static HostApplicationBuilder Configure(
        this HostApplicationBuilder hostApplicationBuilder, Action<CommandLineConfiguration> configure) {
        var configuration = new CommandLineConfiguration();

        configure(configuration);

        hostApplicationBuilder.Services.AddSingleton(configuration);

        return hostApplicationBuilder;
    }
}
