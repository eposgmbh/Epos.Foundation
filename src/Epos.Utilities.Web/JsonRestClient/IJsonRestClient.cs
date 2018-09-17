using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Epos.Utilities.Web
{
    /// <summary> Provides a JSON Web API REST Client. </summary>
    public interface IJsonRestClient
    {
        /// <summary> Gets the Service root URI. </summary>
        string RootUri { get; }

        /// <summary> Executes a GET request and expects back a JSON
        /// response that can be deserialized to a <typeparamref name="TResponse"/>
        /// instance. </summary>
        /// <typeparam name="TResponse">Response type</typeparam>
        /// <param name="apiUri">API URI</param>
        /// <param name="queryParams">Query parameters</param>
        /// <returns>Tuple of <see cref="HttpStatusCode"/> and <typeparamref name="TResponse"/></returns>
        Task<(HttpStatusCode StatusCode, TResponse Response)> GetAsync<TResponse>(
            string apiUri,
            params (string paramName, object paramValue)[] queryParams
        ) where TResponse : class;

        /// <summary> Executes a GET request and expects back a JSON
        /// response that can be deserialized to an
        /// <see cref="System.Collections.Generic.IEnumerable{T}"/> of
        /// <typeparamref name="TResponse"/>. </summary>
        /// <typeparam name="TResponse">Response type</typeparam>
        /// <param name="apiUri">API URI</param>
        /// <param name="queryParams">Query parameters</param>
        /// <returns>Tuple of <see cref="HttpStatusCode"/> and IEnumerable <typeparamref name="TResponse"/></returns>
        Task<(HttpStatusCode StatusCode, IEnumerable<TResponse> Response)> GetManyAsync<TResponse>(
            string apiUri,
            params (string paramName, object paramValue)[] queryParams
        ) where TResponse : class;

        /// <summary> Executes a POST request. </summary>
        /// <param name="apiUri">API URI</param>
        /// <param name="body">Message body (is converted to JSON)</param>
        /// <param name="queryParams">Query parameters</param>
        /// <returns><see cref="HttpStatusCode"/></returns>
        Task<(HttpStatusCode StatusCode, HttpResponseMessage ResponseMessage)> PostAsync(
            string apiUri, object body,
            params (string paramName, object paramValue)[] queryParams
         );

        /// <summary> Executes a PUT request. </summary>
        /// <param name="apiUri">API URI</param>
        /// <param name="body">Message body (is converted to JSON)</param>
        /// <param name="queryParams">Query parameters</param>
        /// <returns><see cref="HttpStatusCode"/></returns>
        Task<(HttpStatusCode StatusCode, HttpResponseMessage ResponseMessage)> PutAsync(
            string apiUri, object body,
            params (string paramName, object paramValue)[] queryParams
        );

        /// <summary> Executes a DELETE request. </summary>
        /// <param name="apiUri">API URI</param>
        /// <param name="queryParams">Query parameters</param>
        /// <returns><see cref="HttpStatusCode"/></returns>
        Task<(HttpStatusCode StatusCode, HttpResponseMessage ResponseMessage)> DeleteAsync(
            string apiUri, params (string paramName, object paramValue)[] queryParams
        );
    }
}
