using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Epos.Utilities.Web
{
    /// <summary> Provides a simple (non-resilient) JSON Web API REST Client. </summary>
    public sealed class JsonRestClient : IJsonRestClient
    {
        private static readonly JsonSerializerSettings DefaultJsonSerializerSettings = new JsonSerializerSettings {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        /// <summary> Initializes a new instance of the <see cref="JsonRestClient"/>
        /// class. </summary>
        /// <param name="rootUri">Service root URI</param>
        public JsonRestClient(string rootUri) {
            if (rootUri == null) {
                throw new ArgumentNullException(nameof(rootUri));
            }

            RootUri = rootUri.Trim();

            if (string.IsNullOrEmpty(RootUri)) {
                throw new ArgumentException("rootUrl cannot be an empty string.");
            }

            if (!RootUri.EndsWith("/")) {
                RootUri += "/";
            }

        }

        /// <inheritdoc />
        public string RootUri { get; }

        /// <inheritdoc />
        public async Task<(HttpStatusCode StatusCode, TResponse Response)> GetAsync<TResponse>(
            string apiUri,
            params (string paramName, object paramValue)[] queryParams
        ) where TResponse : class {
            HttpResponseMessage theResponseMessage = await GetResponseMessage(
                apiUri, queryParams,
                (client, uri) => client.GetAsync(uri)
            );

            return await GetDeserializedJsonObject<TResponse>(theResponseMessage);
        }

        /// <inheritdoc />
        public async Task<(HttpStatusCode StatusCode, IEnumerable<TResponse> Response)> GetManyAsync<TResponse>(
            string apiUri,
            params (string paramName, object paramValue)[] queryParams
        ) where TResponse : class {
            HttpResponseMessage theResponseMessage = await GetResponseMessage(
                apiUri, queryParams,
                (client, uri) => client.GetAsync(uri)
            );

            return await GetDeserializedJsonObject<IEnumerable<TResponse>>(theResponseMessage);
        }

        /// <inheritdoc />
        public async Task<(HttpStatusCode StatusCode, HttpResponseMessage ResponseMessage)> PostAsync(
            string apiUri, object body,
            params (string paramName, object paramValue)[] queryParams
        ) {
            if (body == null) {
                throw new ArgumentNullException(nameof(body));
            }

            HttpResponseMessage theResponseMessage = await GetResponseMessage(
                apiUri, queryParams,
                (client, uri) => client.PostAsync(
                    uri,
                    new StringContent(
                        JsonConvert.SerializeObject(body, Formatting.Indented, DefaultJsonSerializerSettings),
                        Encoding.UTF8, "application/json"
                    )
                )
            );

            return (theResponseMessage.StatusCode, theResponseMessage);
        }

        /// <inheritdoc />
        public async Task<(HttpStatusCode StatusCode, HttpResponseMessage ResponseMessage)> PutAsync(
            string apiUri, object body,
            params (string paramName, object paramValue)[] queryParams
        ) {
            if (body == null) {
                throw new ArgumentNullException(nameof(body));
            }

            HttpResponseMessage theResponseMessage = await GetResponseMessage(
                apiUri, queryParams,
                (client, uri) => client.PutAsync(
                    uri,
                    new StringContent(
                        JsonConvert.SerializeObject(body, Formatting.Indented, DefaultJsonSerializerSettings),
                        Encoding.UTF8, "application/json"
                    )
                )
            );

            return (theResponseMessage.StatusCode, theResponseMessage);
        }

        /// <inheritdoc />
        public async Task<(HttpStatusCode StatusCode, HttpResponseMessage ResponseMessage)> DeleteAsync(
            string apiUri, params (string paramName, object paramValue)[] queryParams
        ) {
            var theReponseMessage = await GetResponseMessage(
                apiUri, queryParams,
                (client, uri) => client.DeleteAsync(uri)
            );

            return (theReponseMessage.StatusCode, theReponseMessage);
        }

        #region --- Helper methods ---

        private async Task<HttpResponseMessage> GetResponseMessage(
            string apiUri, (string paramName, object paramValue)[] queryParams,
            Func<HttpClient, string, Task<HttpResponseMessage>> func
        ) {
            TestApiUri(apiUri);
            TestQueryParams(queryParams);

            using (var theClient = new HttpClient()) {
                theClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string theFullApiUriIncludingQueryParams = GetFullApiUriIncludingQueryParams(apiUri, queryParams);

                return await func(theClient, theFullApiUriIncludingQueryParams);
            }
        }

        private static async Task<(HttpStatusCode StatusCode, TResponse Response)> GetDeserializedJsonObject<TResponse>(
            HttpResponseMessage responseMessage
        ) where TResponse : class {
            TResponse theReponse;
            if (!responseMessage.IsSuccessStatusCode) {
                theReponse = null;
            } else {
                string theJsonResponseString = await responseMessage.Content.ReadAsStringAsync();
                theReponse = JsonConvert.DeserializeObject<TResponse>(
                    theJsonResponseString, DefaultJsonSerializerSettings
                );
            }

            return (responseMessage.StatusCode, theReponse);
        }

        private static void TestApiUri(string apiUri) {
            if (apiUri == null) {
                throw new ArgumentNullException(nameof(apiUri));
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

        private string GetFullApiUriIncludingQueryParams(
            string apiUri, (string paramName, object paramValue)[] queryParams
        ) {
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

            UriBuilder theUriBuilder = new UriBuilder(GetFullApiUri(apiUri)) { Query = theQueryString };
            return theUriBuilder.ToString();
        }

        private string GetFullApiUri(string apiUri) {
            string theApiUri = apiUri.Trim();
            while (theApiUri.StartsWith("/")) {
                theApiUri = theApiUri.Substring(1);
            }

            return RootUri + theApiUri;
        }

        #endregion
    }
}
