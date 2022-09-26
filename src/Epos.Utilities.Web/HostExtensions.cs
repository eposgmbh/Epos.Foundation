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
    internal class LifeTime { }

    /// <summary> Waits for the availability of the supplied Service URLs. After the provided timeout an exception is
    /// thrown. </summary>
    /// <remarks> Also supportes connection strings with "Server=" and "Port=" components. </remarks>
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
        IHostEnvironment theEnvironment = theScope.ServiceProvider.GetRequiredService<IHostEnvironment>();

        foreach (var theConfigurationEntry in serviceUrlConfigurationEntries) {
            if (theConfigurationEntry is null) {
                throw new ArgumentNullException(nameof(serviceUrlConfigurationEntries));
            }

            string theServiceUrl = theConfiguration[theConfigurationEntry];

            WaitForServiceAvailability(theServiceUrl, timeoutSeconds, theEnvironment.IsDevelopment(), theLogger);
        }
    }

    internal static void WaitForServiceAvailability(string serviceUrl, int timeoutSeconds, bool isDevelopment, ILogger logger) {
        var theClient = new TcpClient();
        var theStopwatch = Stopwatch.StartNew();

        var (theHost, thePort) = GetHostAndPort(serviceUrl);

        bool theIsSuccessful = false;
        do {
            try {
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
        } else {
            logger.LogInformation(
                $"Host {theHost}:{thePort} is available after {theStopwatch.ElapsedMilliseconds} milliseconds."
            );

            if (!isDevelopment) {
                // Let's wait another second for non-dev environments
                Thread.Sleep(millisecondsTimeout: 1000);
            }
        }
    }

    #region --- Helper methods ---

    private static (string Host, int Port) GetHostAndPort(string serviceUrl) {
        try {
            Uri theUri = new Uri(serviceUrl);
            return (theUri.Host, theUri.Port);
        } catch (UriFormatException) {
            var theRegex = new Regex(@"\s*[S|s]erver=([A-Za-z0-9\.\-]+).*[P|p]ort=([0-9]+).*", RegexOptions.Compiled);

            var theMatch = theRegex.Matches(serviceUrl).SingleOrDefault();
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
