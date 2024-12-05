using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Epos.Utilities.Web;

/// <summary> Collection of <b>Host</b> extension methods. </summary>
public static class HostExtensions
{
    /// <summary> Waits for the availability of the supplied Service URLs. After the provided timeout an exception is
    /// thrown. </summary>
    /// <remarks> Also supports connection strings with "Server=" and "Port=" components. </remarks>
    /// <param name="host">Host</param>
    /// <param name="timeoutSeconds">Timeout in seconds</param>
    /// <param name="serviceUrlConfigurationEntries">Names of the Service URL configuration entries</param>
    /// <returns> Host </returns>
    /// <exception cref="TimeoutException">Thrown when the timeout has run out.</exception>
    public static IHost WaitForServiceAvailability(
        this IHost host, int timeoutSeconds, params string[] serviceUrlConfigurationEntries
    ) {
        if (host is null) {
            throw new ArgumentNullException(nameof(host));
        }

        using IServiceScope theScope = host.Services.CreateScope();
        IServiceProvider theServiceProvider = theScope.ServiceProvider;

        WaitForServiceAvailability(theServiceProvider, timeoutSeconds, serviceUrlConfigurationEntries);

        return host;
    }

    /// <summary> Waits for the availability of the supplied Service URLs. After the provided timeout an exception is
    /// thrown. </summary>
    /// <remarks> Also supports connection strings with "Server=" and "Port=" components. </remarks>
    /// <param name="services">Service collection</param>
    /// <param name="timeoutSeconds">Timeout in seconds</param>
    /// <param name="serviceUrl">Service URL</param>
    /// <exception cref="TimeoutException">Thrown when the timeout has run out.</exception>
    public static void WaitForServiceAvailability(
        this IServiceCollection services, int timeoutSeconds, string serviceUrl
    ) {
        if (services is null) {
            throw new ArgumentNullException(nameof(services));
        }

        IServiceProvider theServiceProvider = services.BuildServiceProvider();
        ILogger theLogger = theServiceProvider.GetRequiredService<ILogger<Logging>>();

        WaitForServiceAvailability(serviceUrl, theLogger, timeoutSeconds);
    }

    internal static void WaitForServiceAvailability(string serviceUrl, ILogger logger, int timeoutSeconds) {
        var theClient = new TcpClient();
        var theStopwatch = Stopwatch.StartNew();

        (string theHost, int thePort) = GetHostAndPort(serviceUrl);

        bool theIsSuccessful = false;
        bool theIsLogged = false;
        do {
            try {
                if (!theIsLogged && theStopwatch.ElapsedMilliseconds >= 2000) {
                    logger.LogInformation(
                        $"Waiting for the availability of the host {theHost}:{thePort}..."
                    );
                    theIsLogged = true;
                }

                theClient.Connect(theHost, thePort);
                theIsSuccessful = true;
            } catch (SocketException) {
                Thread.Sleep(millisecondsTimeout: 500);
            }
        } while (!theIsSuccessful && theStopwatch.Elapsed.Seconds < timeoutSeconds);

        theClient.Close();

        if (!theIsSuccessful) {
            throw new TimeoutException(
                $"Host {theHost}:{thePort} is not available after {timeoutSeconds} seconds."
            );
        }
        
        long theElapsedMilliseconds = theStopwatch.ElapsedMilliseconds;

        logger.LogInformation(
            $"Host {theHost}:{thePort} is available after {theElapsedMilliseconds} milliseconds."
        );

        if (theElapsedMilliseconds >= 1000) {
            logger.LogInformation(
                "Waiting an additional 5 seconds for initialization..."
            );
    
            Thread.Sleep(millisecondsTimeout: 5000);
        }
    }

    #region --- Helper methods ---

    private static void WaitForServiceAvailability(
        this IServiceProvider serviceProvider, int timeoutSeconds, string[] serviceUrlConfigurationEntries
    ) {
        if (serviceProvider is null) {
            throw new ArgumentNullException(nameof(serviceProvider));
        }
        if (serviceUrlConfigurationEntries is null) {
            throw new ArgumentNullException(nameof(serviceUrlConfigurationEntries));
        }

        IConfiguration theConfiguration = serviceProvider.GetRequiredService<IConfiguration>();
        ILogger theLogger = serviceProvider.GetRequiredService<ILogger<Logging>>();

        foreach (string? theConfigurationEntry in serviceUrlConfigurationEntries) {
            if (theConfigurationEntry is null) {
                throw new ArgumentNullException(nameof(serviceUrlConfigurationEntries));
            }

            string? theServiceUrl = theConfiguration[theConfigurationEntry];

            WaitForServiceAvailability(theServiceUrl!, theLogger, timeoutSeconds);
        }
    }

    private static (string Host, int Port) GetHostAndPort(string serviceUrl) {
        try {
            var theUri = new Uri(serviceUrl);
            return (theUri.Host, theUri.Port);
        } catch (UriFormatException) {
            var theRegex = new Regex(@"\s*[S|s]erver=([A-Za-z0-9\.\-]+).*[P|p]ort=([0-9]+).*", RegexOptions.Compiled);

            Match? theMatch = theRegex.Matches(serviceUrl).SingleOrDefault();
            if (theMatch is not null) {
                string theHost = theMatch.Groups[1].Value;
                if (int.TryParse(theMatch.Groups[2].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int thePort)) {
                    return (theHost, thePort);
                }
            }

            throw new ArgumentException($"Invalid service URL or connection string: {serviceUrl}", nameof(serviceUrl));
        }
    }

    #endregion
}
