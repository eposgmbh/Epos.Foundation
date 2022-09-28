using System;
using System.Net.Http;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Polly;
using Polly.Extensions.Http;

namespace Epos.Utilities.Web;

/// <summary> Collection of <b>HttpClientBuilder</b> extension methods. </summary>
public static class HttpClientBuilderExtensions
{
    private const int RetryCount = 6;
    private const int Seconds = 2;

    /// <summary> Adds resiliency for an <b>HttpClient.</b> </summary>
    /// <param name="httpClientBuilder">HTTP Client Builder</param>
    /// <param name="baseUrl">Base URL for HTTP Client (can be <b>null</b>)</param>
    /// <returns> HttpClientBuilder </returns>
    public static IHttpClientBuilder AddResiliency(this IHttpClientBuilder httpClientBuilder, string? baseUrl = null) {
        if (httpClientBuilder is null) {
            throw new ArgumentNullException(nameof(httpClientBuilder));
        }

        if (baseUrl is null) {
            httpClientBuilder.AddPolicyHandler(
                HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(
                        retryCount: RetryCount,
                        sleepDurationProvider: SleepDurationProvider
                    )
            );
        } else {
            httpClientBuilder.AddPolicyHandler((services, request) =>
                HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(
                        retryCount: RetryCount,
                        sleepDurationProvider: SleepDurationProvider,
                        onRetry: (outcome, timespan, retryAttempt, context) =>
                            services.GetRequiredService<ILogger<HttpClient>>()
                                .LogWarning(
                                    "{baseUrl}: Delaying for {delay} ms, then making retry {retry}.",
                                    baseUrl, timespan.TotalMilliseconds, retryAttempt
                                )
                    )
            );
        }

        return httpClientBuilder;
    }

    /// <summary> Allows untrusted certificates for an <b>HttpClient.</b> </summary>
    /// <remarks> Should only be used for development scenarios. </remarks>
    /// <param name="httpClientBuilder">HTTP Client Builder</param>
    /// <returns> HttpClientBuilder </returns>
    public static IHttpClientBuilder AllowUntrustedCertificates(this IHttpClientBuilder httpClientBuilder) {
        if (httpClientBuilder is null) {
            throw new ArgumentNullException(nameof(httpClientBuilder));
        }

        using IServiceScope theServiceScope = httpClientBuilder.Services.BuildServiceProvider().CreateScope();
        ILogger theLogger = theServiceScope.ServiceProvider.GetRequiredService<ILogger<HttpClient>>();

        HttpClientHandler theHttpClientHandler = new HttpClientHandler {
            ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => {
                theLogger.LogWarning($"Trusting certificate {cert!.Issuer}.");
                return true;
            }
        };

        httpClientBuilder.ConfigurePrimaryHttpMessageHandler(() => theHttpClientHandler);

        return httpClientBuilder;
    }

    private static TimeSpan SleepDurationProvider(int retryAttempt) {
        return TimeSpan.FromSeconds(Math.Pow(Seconds, retryAttempt));
    }
}
