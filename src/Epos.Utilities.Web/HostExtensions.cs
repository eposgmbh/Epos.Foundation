using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Epos.Utilities.Web;

/// <summary> Collection of <b>Host</b> extension methods. </summary>
public static class HostExtensions
{
    internal class LifeTime { }

    /// <summary> Waits for the availability of the supplied Service URLs. After the provided timeout an exception is
    /// thrown. </summary>
    /// <param name="host">Host</param>
    /// <param name="timeoutSeconds">Timeout in seconds</param>
    /// <param name="serviceUrlConfigurationEntries">Names of the Service URL configuration entries</param>
    /// <exception cref="TimeoutException">Thrown when the timeout has run out.</exception>
    public static void WaitForServiceAvailability(
        this IHost host, int timeoutSeconds, params string[] serviceUrlConfigurationEntries
    ) {
        if (host is null) {
            throw new ArgumentNullException(nameof(host));
        }
        if (serviceUrlConfigurationEntries is null) {
            throw new ArgumentNullException(nameof(serviceUrlConfigurationEntries));
        }

        using IServiceScope theScope = host.Services.CreateScope();
        IConfiguration theConfiguration = theScope.ServiceProvider.GetRequiredService<IConfiguration>();
        ILogger theLogger = theScope.ServiceProvider.GetRequiredService<ILogger<LifeTime>>();

        foreach (var theConfigurationEntry in serviceUrlConfigurationEntries) {
            if (theConfigurationEntry is null) {
                throw new ArgumentNullException(nameof(serviceUrlConfigurationEntries));
            }

            Uri theUri = new Uri(theConfiguration[theConfigurationEntry]);

            var theClient = new TcpClient();
            var theStopwatch = Stopwatch.StartNew();

            bool theIsSuccessful = false;
            do {
                try {
                    theClient.Connect(theUri.Host, theUri.Port);
                    theIsSuccessful = true;
                } catch (SocketException) {
                    Thread.Sleep(millisecondsTimeout: 500);
                }
            } while (!theIsSuccessful && theStopwatch.Elapsed.Seconds < timeoutSeconds);

            theClient.Close();

            if (!theIsSuccessful) {
                throw new TimeoutException(
                    $"Host {theUri.Host}:{theUri.Port} is not available after {timeoutSeconds} seconds."
                );
            } else {
                theLogger.LogInformation(
                    $"Host {theUri.Host}:{theUri.Port} is available after {theStopwatch.ElapsedMilliseconds} " +
                    $"milliseconds."
                );

                IHostEnvironment theEnvironment = theScope.ServiceProvider.GetRequiredService<IHostEnvironment>();
                if (!theEnvironment.IsDevelopment()) {
                    // Let's wait another second for non-dev environments
                    Thread.Sleep(millisecondsTimeout: 1000);
                }
            }
        }
    }
}
