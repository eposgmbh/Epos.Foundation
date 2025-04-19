using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;

using Epos.Utilities;

namespace Epos.CommandLine.Helpers;

/// <summary> Provides helper methods for any Refit REST Client. </summary>
public static class RefitRestClientConsoleHelper
{
    /// <summary> Executes a provided REST client and a success action, if the REST client succeeds.
    /// </summary>
    /// <remarks> Pretty-prints error messages for common HTTP status codes. </remarks>
    /// <param name="refitRestClientFunc">REST client function</param>
    /// <param name="successAction">Success action that gets the reponse from the REST client</param>
    /// <returns>Exit code</returns>
    public static async Task<int> ExecuteAsync<T>(
        Func<Task<T>> refitRestClientFunc,
        Func<T, int> successAction
    ) {
        ArgumentNullException.ThrowIfNull(refitRestClientFunc);
        ArgumentNullException.ThrowIfNull(successAction);

        try {
            T theResponse = await refitRestClientFunc();

            Console.WriteLine();

            return successAction(theResponse);
        } catch (Exception theException) {
            return HandleException(theException);
        }
    }

    /// <summary> Executes a provided REST client and a success action, if the REST client succeeds.
    /// </summary>
    /// <remarks> Pretty-prints error messages for common HTTP status codes. </remarks>
    /// <param name="refitRestClientFunc">REST client function</param>
    /// <param name="successAction">Success action that gets the reponse from the REST client</param>
    /// <returns>Exit code</returns>
    public static async Task<int> ExecuteAsync(
        Func<Task> refitRestClientFunc,
        Func<int> successAction
    ) {
        ArgumentNullException.ThrowIfNull(refitRestClientFunc);
        ArgumentNullException.ThrowIfNull(successAction);

        try {
            await refitRestClientFunc();

            Console.WriteLine();

            return successAction();
        } catch (Exception theException) {
            return HandleException(theException);
        }
    }

    #region --- Helper methods ---

    private static int HandleException(Exception exception) {
        if (exception is AggregateException theAggregateException) {
            exception = theAggregateException.InnerExceptions.First();
        }

        if (exception.GetType().FullName == "Refit.ValidationApiException") {
            return HandleValidationApiException(exception);
        }

        if (exception.GetType().FullName == "Refit.ApiException") {
            return HandleApiException(exception);
        }

        Console.WriteLine();
        ColorConsole.WriteError(" ERROR ");
        Console.WriteLine($" {exception.Message}");

        return -1;
    }
    [UnconditionalSuppressMessage("AssemblyLoadTrimming", "IL2075", Justification = "Types are available at runtime.")]
    private static int HandleValidationApiException(Exception validationApiException) {
        Console.WriteLine();

        object theContent = validationApiException
            .GetType()
            .GetProperty("Content", BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)!
            .GetGetMethod()!
            .Invoke(validationApiException, [])!;

        string theDetail = (string) theContent
            .GetType()
            .GetProperty("Detail")!
            .GetGetMethod()!
            .Invoke(theContent, [])!;

        var theErrors = (Dictionary<string, string[]>) theContent
            .GetType()
            .GetProperty("Errors")!
            .GetGetMethod()!
            .Invoke(theContent, [])!;

        var theErrorStrings = theErrors.SelectMany(e => e.Value).ToList();

        ColorConsole.WriteError(" ERROR ");

        if (theErrorStrings.Count != 0) {
            Console.WriteLine($" {theErrorStrings.First()}");
        } else {
            Console.WriteLine($" {theDetail}");
        }

        return 6;
    }

    [UnconditionalSuppressMessage("AssemblyLoadTrimming", "IL2075", Justification = "Types are available at runtime.")]
    private static int HandleApiException(Exception apiException) {
        Console.WriteLine();

        var theStatusCode = (HttpStatusCode) apiException
            .GetType()
            .GetProperty("StatusCode")!
            .GetGetMethod()!
            .Invoke(apiException, [])!;

        if (theStatusCode == HttpStatusCode.Unauthorized) {
            ColorConsole.WriteError(" ERROR ");
            Console.WriteLine(" Invalid or expired token.");

            return 1;
        } else if (theStatusCode == HttpStatusCode.Forbidden) {
            ColorConsole.WriteError(" ERROR ");
            Console.WriteLine(" You are not authorized to execute this command.");

            return 2;
        } else if (theStatusCode == HttpStatusCode.NotFound) {
            var theRequestMessage = (HttpRequestMessage) apiException
                .GetType()
                .GetProperty("RequestMessage")!
                .GetGetMethod()!
                .Invoke(apiException, [])!;

            ColorConsole.WriteError(" ERROR ");
            Console.WriteLine($" Not found. Invalid URL? ({theRequestMessage.Method} {theRequestMessage.RequestUri})");

            return 3;
        } else {
            var theContentHeaders = (HttpContentHeaders?) apiException
                .GetType()
                .GetProperty("ContentHeaders")!
                .GetGetMethod()!
                .Invoke(apiException, []);

            if (theContentHeaders?.ContentType?.MediaType == "text/html") {
                ColorConsole.WriteError(" ERROR ");
                Console.WriteLine(" Cannot connect to host (text/html proxy found, but no json REST response).");

                return 4;
            } else {
                string? theContent = (string?) apiException
                    .GetType()
                    .GetProperty("Content")!
                    .GetGetMethod()!
                    .Invoke(apiException, []);

                ColorConsole.WriteError(" ERROR ");
                Console.WriteLine($" {theContent}");

                return 5;
            }
        }
    }

    #endregion
}
