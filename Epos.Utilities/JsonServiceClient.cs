using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Epos.Utilities
{
    /// <summary> Simple JSON Web API Client. </summary>
    public sealed class JsonServiceClient
    {
        private static readonly JsonSerializerSettings DefaultJsonSerializerSettings = new JsonSerializerSettings {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        /// <summary> Initializes a new instance of the <see cref="JsonServiceClient"/>
        /// class. </summary>
        /// <param name="rootUrl">Service root URL</param>
        public JsonServiceClient(string rootUrl) {
            if (rootUrl == null) {
                throw new ArgumentNullException(nameof(rootUrl));
            }

            RootUrl = rootUrl.Trim();

            if (string.IsNullOrEmpty(RootUrl)) {
                throw new ArgumentException("rootUrl cannot be an empty string.");
            }

            if (!RootUrl.EndsWith("/")) {
                RootUrl += "/";
            }

        }

        /// <summary> Gets the Service root URL. </summary>
        public string RootUrl { get; }

        /// <summary> Executes a GET request and expects back a JSON
        /// response that can be deserialized to a <typeparamref name="TResponse"/>
        /// instance. </summary>
        /// <typeparam name="TResponse">Response type</typeparam>
        /// <param name="apiUrl">API URL</param>
        /// <param name="queryParams">Query parameters</param>
        /// <returns><typeparamref name="TResponse"/> instance</returns>
        public async Task<TResponse> GetOneAsync<TResponse>(
            string apiUrl,
            params (string paramName, object paramValue)[] queryParams
        ) {
            HttpResponseMessage theResponseMessage = await GetResponseMessage(
                apiUrl, queryParams,
                (client, url) => client.GetAsync(url)
            );

            return await GetDeserializedJsonObject<TResponse>(theResponseMessage);
        }

        /// <summary> Executes a GET request and expects back a JSON
        /// response that can be deserialized to an
        /// <see cref="System.Collections.Generic.IEnumerable{T}"/> of
        /// <typeparamref name="TResponse"/>. </summary>
        /// <typeparam name="TResponse">Response type</typeparam>
        /// <param name="apiUrl">API URL</param>
        /// <param name="queryParams">Query parameters</param>
        /// <returns><see cref="System.Collections.Generic.IEnumerable{T}"/> of
        /// <typeparamref name="TResponse"/> instance</returns>
        public async Task<IEnumerable<TResponse>> GetManyAsync<TResponse>(
            string apiUrl,
            params (string paramName, object paramValue)[] queryParams
        ) where TResponse : class {
            HttpResponseMessage theResponseMessage = await GetResponseMessage(
                apiUrl, queryParams,
                (client, url) => client.GetAsync(url)
            );

            return await GetDeserializedJsonObject<IEnumerable<TResponse>>(theResponseMessage);
        }

        /// <summary> Executes a GET request and expects back a JSON
        /// response that can be deserialized to a <typeparamref name="TResponse"/>
        /// instance. </summary>
        /// <typeparam name="TResponse">Response type</typeparam>
        /// <param name="apiUrl">API URL</param>
        /// <param name="body">Message body (is converted to JSON)</param>
        /// <param name="queryParams">Query parameters</param>
        /// <returns><typeparamref name="TResponse"/> instance</returns>
        public async Task<TResponse> PostAsync<TResponse>(
            string apiUrl, object body,
            params (string paramName, object paramValue)[] queryParams
        ) where TResponse : class {
            if (body == null) {
                throw new ArgumentNullException(nameof(body));
            }

            HttpResponseMessage theResponseMessage = await GetResponseMessage(
                apiUrl, queryParams,
                (client, url) => client.PostAsync(
                    url,
                    new StringContent(
                        JsonConvert.SerializeObject(body, Formatting.Indented, DefaultJsonSerializerSettings),
                        Encoding.UTF8, "application/json"
                    )
                )
            );

            return await GetDeserializedJsonObject<TResponse>(theResponseMessage);
        }

        #region --- Hilfsmethoden ---

        private async Task<HttpResponseMessage> GetResponseMessage(
            string apiUrl, (string paramName, object paramValue)[] queryParams,
            Func<HttpClient, string, Task<HttpResponseMessage>> func
        ) {
            TestApiUrl(apiUrl);
            TestQueryParams(queryParams);

            using (var theClient = new HttpClient()) {
                theClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string theFullApiUrlIncludingQueryParams = GetFullApiUrlIncludingQueryParams(apiUrl, queryParams);

                return await func(theClient, theFullApiUrlIncludingQueryParams);
            }
        }

        private static async Task<TResponse> GetDeserializedJsonObject<TResponse>(
            HttpResponseMessage responseMessage
        ) {
            responseMessage.EnsureSuccessStatusCode();

            string theJsonResponseString = await responseMessage.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResponse>(theJsonResponseString, DefaultJsonSerializerSettings);
        }

        private static void TestApiUrl(string apiUrl) {
            if (apiUrl == null) {
                throw new ArgumentNullException(nameof(apiUrl));
            }
        }

        private static void TestQueryParams((string paramName, object paramValue)[] queryParams) {
            if (queryParams == null) {
                throw new ArgumentNullException(nameof(queryParams));
            }

            for (int theIndex = 0; theIndex < queryParams.Length; theIndex++) {
                var theQueryParam = queryParams[theIndex];

                if (theQueryParam.paramName == null) {
                    throw new ArgumentNullException("queryParams[" + theIndex + "].paramName");
                }
                if (theQueryParam.paramValue == null) {
                    throw new ArgumentNullException("queryParams[" + theIndex + "].paramValue");
                }
            }
        }

        private string GetFullApiUrlIncludingQueryParams(string apiUrl, (string paramName, object paramValue)[] queryParams) {
            string theQueryString;
            if (queryParams.Length > 0) {
                var theContent = new FormUrlEncodedContent(
                    queryParams.Select(qp => new KeyValuePair<string, string>(qp.paramName, qp.paramValue.ToString()))
                );
                theQueryString = theContent.ReadAsStringAsync().Result;
                theContent.Dispose();
            } else {
                theQueryString = null;
            }

            UriBuilder theUriBuilder = new UriBuilder(GetFullApiUrl(apiUrl)) { Query = theQueryString };
            return theUriBuilder.ToString();
        }

        private string GetFullApiUrl(string apiUrl) {
            string theApiUrl = apiUrl.Trim();
            while (theApiUrl.StartsWith("/")) {
                theApiUrl = theApiUrl.Substring(1);
            }

            return RootUrl + theApiUrl;
        }

        #endregion
    }
}
