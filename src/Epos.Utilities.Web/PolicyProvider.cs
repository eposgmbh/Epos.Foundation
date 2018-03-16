using System;
using System.Collections.Generic;
using System.Net.Http;

using Microsoft.Extensions.Logging;

using Polly;
using Polly.Wrap;

namespace Epos.Utilities.Web
{
    /// <inheritdoc />
    public class PolicyProvider : IPolicyProvider
    {
        private readonly Dictionary<string, PolicyWrap> myPolicies = new Dictionary<string, PolicyWrap>();
        private readonly ILogger<PolicyProvider> myLogger;

        /// <summary> Initializes a new instance of the <see cref="PolicyProvider"/>
        /// class. </summary>
        /// <param name="logger">Logger</param>
        public PolicyProvider(ILogger<PolicyProvider> logger = null) {
            myLogger = logger;
        }

        /// <summary> Provides a Polly policy with retryCount 5, exponential back off and a circuit breaker
        /// allowing 5 exceptions. </summary>
        /// <param name="url">URL for caching</param>
        /// <returns></returns>
        public PolicyWrap ProvidePolicy(string url) {
            var theUrl = new Uri(url);
            var origin = $"{theUrl.Scheme}://{theUrl.DnsSafeHost}:{theUrl.Port}";
            origin = origin.ToLower();

            if (!myPolicies.TryGetValue(origin, out PolicyWrap thePolicyWrap)) {
                thePolicyWrap = Policy.WrapAsync(CreatePolicies());
                myPolicies[origin] = thePolicyWrap;
            }

            return thePolicyWrap;
        }

        #region --- Helper methods ---

        private Policy[] CreatePolicies() => new Policy[] {
            Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(
                    retryCount: 5,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (exception, timeSpan, retryCount, context) => {
                        myLogger?.LogWarning(
                            $"Retry {retryCount} implemented with Polly's RetryPolicy " +
                            $"of {context.PolicyKey} " +
                            $"at {context.ExecutionKey}, " +
                            $"due to: {exception}."
                        );
                    }
                ),

            Policy
                .Handle<HttpRequestException>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromMinutes(1),
                    onBreak: (exception, duration) => {
                        myLogger?.LogTrace($"Circuit breaker opened with message \"{exception.Message}\"");
                    },
                    onReset: () => {
                        myLogger?.LogTrace("Circuit breaker reset");
                    }
                )
        };

        #endregion
    }
}
