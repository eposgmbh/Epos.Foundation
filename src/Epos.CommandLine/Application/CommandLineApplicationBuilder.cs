using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Epos.CommandLine.Application;

/// <summary> Creates a <b>HostApplicationBuilder</b> that simplifies using Epos.CommandLine
/// in an application host with configuration, logging and dependency injection. </summary>
public static class CommandLineApplicationBuilder
{
    internal static string[] Args = [];

    /// <summary> Creates the application builder with pre-configured defaults.
    /// </summary>
    /// <param name="args">Command line arguments</param>
    /// <returns>The initialized <b>HostApplicationBuilder</b></returns>
    public static HostApplicationBuilder CreateApplicationBuilder(string[] args) {
        ArgumentNullException.ThrowIfNull(args);

        HostApplicationBuilder result = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings {
            Args = args,
            EnvironmentName = Environments.Production,
            ContentRootPath = Path.GetDirectoryName(AppContext.BaseDirectory)
        });

        // Set custom logger
        result.Logging.ClearProviders();
        result.Logging.AddProvider(new LoggerProvider());
        result.Logging.SetMinimumLevel(LogLevel.Trace);

        // Disable "Microsoft.Hosting.Lifetime" info logging
        result.Logging.AddFilter("Microsoft", LogLevel.Warning);

        Args = args;

        result.Services.AddHostedService<ConsoleHostedService>();

        return result;
    }
}
