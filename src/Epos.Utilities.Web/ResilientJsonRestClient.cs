using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;

namespace Epos.Utilities.Web
{
    /// <summary> Provides a resilient JSON Web API REST Client. </summary>
    public class ResilientJsonRestClient : IJsonRestClient
    {
        private readonly IPolicyProvider myPolicyProvider;
        private readonly JsonRestClient myInternalClient;

        /// <summary> Initializes a new instance of the <see cref="ResilientJsonRestClient"/>
        /// class. </summary>
        /// <param name="policyProvider">Policy provider</param>
        /// <param name="rootUri">Service root URL</param>
        public ResilientJsonRestClient(IPolicyProvider policyProvider, string rootUri) {
            if (policyProvider == null) {
                throw new ArgumentNullException(nameof(policyProvider));
            }

            myPolicyProvider = policyProvider;
            myInternalClient = new JsonRestClient(rootUri);
        }

        /// <inheritdoc />
        public string RootUri => myInternalClient.RootUri;

        /// <inheritdoc />
        public Task<(HttpStatusCode StatusCode, TResponse Response)> GetAsync<TResponse>(
            string apiUri, params (string paramName, object paramValue)[] queryParams
        ) where TResponse : class {
            var thePolicy = myPolicyProvider.ProvidePolicy(RootUri);
            return thePolicy.ExecuteAsync(
                async () => {
                    var theResult = await myInternalClient.GetAsync<TResponse>(apiUri, queryParams);
                    ThrowForCircuitBreakerIfNeccessary(theResult.StatusCode);
                    return theResult;
                }, new Context(RootUri)
            );
        }

        /// <inheritdoc />
        public Task<(HttpStatusCode StatusCode, IEnumerable<TResponse> Response)> GetManyAsync<TResponse>(
            string apiUri, params (string paramName, object paramValue)[] queryParams
        ) where TResponse : class {
            var thePolicy = myPolicyProvider.ProvidePolicy(RootUri);
            return thePolicy.ExecuteAsync(
                async () => {
                    var theResult = await myInternalClient.GetManyAsync<TResponse>(apiUri, queryParams);
                    ThrowForCircuitBreakerIfNeccessary(theResult.StatusCode);
                    return theResult;
                }, new Context(RootUri)
            );
        }

        /// <inheritdoc />
        public Task<(HttpStatusCode StatusCode, HttpResponseMessage ResponseMessage)> PostAsync(
            string apiUri, object body, params (string paramName, object paramValue)[] queryParams
        ) {
            var thePolicy = myPolicyProvider.ProvidePolicy(RootUri);
            return thePolicy.ExecuteAsync(
                async () => {
                    var theResult = await myInternalClient.PostAsync(apiUri, body, queryParams);
                    ThrowForCircuitBreakerIfNeccessary(theResult.StatusCode);
                    return theResult;
                }, new Context(RootUri)
            );
        }

        /// <inheritdoc />
        public Task<(HttpStatusCode StatusCode, HttpResponseMessage ResponseMessage)> PutAsync(
            string apiUri, object body, params (string paramName, object paramValue)[] queryParams
        ) {
            var thePolicy = myPolicyProvider.ProvidePolicy(RootUri);
            return thePolicy.ExecuteAsync(
                async () => {
                    var theResult = await myInternalClient.PutAsync(apiUri, body, queryParams);
                    ThrowForCircuitBreakerIfNeccessary(theResult.StatusCode);
                    return theResult;
                }, new Context(RootUri)
            );
        }

        /// <inheritdoc />
        public Task<(HttpStatusCode StatusCode, HttpResponseMessage ResponseMessage)> DeleteAsync(
            string apiUri, params (string paramName, object paramValue)[] queryParams
        ) {
            var thePolicy = myPolicyProvider.ProvidePolicy(RootUri);
            return thePolicy.ExecuteAsync(
                async () => {
                    var theResult = await myInternalClient.DeleteAsync(apiUri, queryParams);
                    ThrowForCircuitBreakerIfNeccessary(theResult.StatusCode);
                    return theResult;
                }, new Context(RootUri)
            );
        }

        #region --- Helper methods ---

        private static void ThrowForCircuitBreakerIfNeccessary(HttpStatusCode statusCode) {
            if ((int)statusCode >= 500 /* Internal Server Errors */ ||
                (int)statusCode == 429 /* Too Many Requests */) {
                throw new HttpRequestException($"Circuit Breaker: {statusCode}");
            }
        }

        #endregion
    }
}
