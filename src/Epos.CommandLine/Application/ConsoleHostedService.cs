using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Epos.CommandLine.Application;

internal sealed class ConsoleHostedService(
    IHostApplicationLifetime hostApplicationLifetime,
    IServiceProvider serviceProvider,
    ILogger<ConsoleHostedService> logger
) : IHostedService
{
    private int myExitCode = byte.MaxValue;

    public async Task StartAsync(CancellationToken cancellationToken) {
        IEnumerable<ISubcommand> theSubcommands = serviceProvider.GetServices<ISubcommand>();
        if (!theSubcommands.Any()) {
            throw new InvalidOperationException("You have not registered any subcommand.");
        }

        bool hasDifferentiatedSubcommands =
            theSubcommands.Count() > 1 || theSubcommands.First().Name != ISubcommand.DefaultName;

        var theCommandLineDefinition = new CommandLineDefinition {
            HasDifferentiatedSubcommands = hasDifferentiatedSubcommands,
        };

        CommandLineConfiguration? theCommandLineConfiguration = serviceProvider.GetService<CommandLineConfiguration>();
        if (theCommandLineConfiguration is not null) {
            theCommandLineDefinition.Configuration = theCommandLineConfiguration;
        }

        foreach (ISubcommand theSubcommand in theSubcommands) {
            theCommandLineDefinition.Subcommands.Add(new CommandLineSubcommandWrapper(theSubcommand));
        }

        try {
            myExitCode = await theCommandLineDefinition.TryAsync(CommandLineApplicationBuilder.Args, cancellationToken);
        } catch (FileNotFoundException theException) {
            logger.LogError("{ErrorMessage}", theException.Message);
        } catch (Exception theException) {
            logger.LogCritical("{ErrorMessage}", theException.Message);
            logger.LogInformation("{StackTrace}", theException.StackTrace);
        }

        hostApplicationLifetime.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        Environment.ExitCode = myExitCode;

        return Task.CompletedTask;
    }
}
