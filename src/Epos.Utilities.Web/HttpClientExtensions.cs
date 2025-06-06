using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;

namespace Epos.Utilities.Web;

/// <summary>
/// Provides extension methods for <see cref="HttpClient"/> to support advanced JSON operations.
/// </summary>
public static class HttpClientExtensions
{
    /// <summary>
    /// Sends a POST request with a JSON body and asynchronously yields the deserialized results as an async enumerable.
    /// </summary>
    /// <typeparam name="TResult">The type of the result elements.</typeparam>
    /// <param name="httpClient">The HTTP client to use for the request.</param>
    /// <param name="requestUri">The URI the request is sent to.</param>
    /// <param name="value">The value to serialize as JSON in the request body.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>An async enumerable of deserialized results.</returns>
    [UnconditionalSuppressMessage("AssemblyLoadTrimming", "IL2026", Justification = "Types are available at runtime.")]
    [UnconditionalSuppressMessage("AssemblyLoadTrimming", "IL3050", Justification = "Types are available at runtime.")]
    public static async IAsyncEnumerable<TResult> PostAsJsonAsyncEnumerable<TResult>(
        this HttpClient httpClient, string? requestUri, object value,
        [EnumeratorCancellation] CancellationToken cancellationToken = default) {
        System.ArgumentNullException.ThrowIfNull(httpClient);
        System.ArgumentNullException.ThrowIfNull(value);

        using var request = new HttpRequestMessage(HttpMethod.Post, requestUri) {
            Content = JsonContent.Create(value)
        };

        using HttpResponseMessage response = await httpClient.SendAsync(
            request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        response.EnsureSuccessStatusCode();

        using Stream stream = await response.Content.ReadAsStreamAsync();

        IAsyncEnumerable<TResult?> results = JsonSerializer.DeserializeAsyncEnumerable<TResult>(
            stream, JsonSerializerOptions.Web, cancellationToken);

        await foreach (TResult? result in results) {
            if (result is not null) {
                yield return result;
            }
        }
    }
}
