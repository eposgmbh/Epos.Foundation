using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Epos.Utilities;

using Refit;

namespace Epos.CmdLine.Helpers
{
    /// <summary> Provides helper methods for any Refit REST Client. </summary>
    public static class RefitRestClientConsoleHelper
    {
        /// <summary> Executes a provided REST client and a success action, if the REST client succeeds.
        /// </summary>
        /// <remarks> Pretty-prints error messages for common HTTP status codes. </remarks>
        /// <param name="refitRestClientFunc">REST client function</param>
        /// <param name="successAction">Success action that gets the reponse from the REST client</param>
        /// <returns>Exit code</returns>
        public static int Execute<T>(
            Func<Task<ApiResponse<T>>> refitRestClientFunc,
            Func<T, int> successAction
        ) {
            if (refitRestClientFunc == null) {
                throw new ArgumentNullException(nameof(refitRestClientFunc));
            }
            if (successAction == null) {
                throw new ArgumentNullException(nameof(successAction));
            }

            try {
                ApiResponse<T> theResponse = refitRestClientFunc().Result;

                Console.WriteLine();

                if (theResponse.StatusCode == HttpStatusCode.Unauthorized) {
                    ColorConsole.WriteError(" ERROR ");
                    Console.WriteLine(" Invalid or expired token.");

                    return 1;
                }
                if (theResponse.StatusCode == HttpStatusCode.Forbidden) {
                    ColorConsole.WriteError(" ERROR ");
                    Console.WriteLine(" You are not authorized to execute this command.");

                    return 2;
                } else if (theResponse.StatusCode == HttpStatusCode.NotFound) {
                    ColorConsole.WriteError(" ERROR ");
                    Console.WriteLine(
                        $" Not found. Invalid URL?" +
                        $" ({theResponse.RequestMessage!.Method} {theResponse.RequestMessage!.RequestUri})"
                    );

                    return 4;
                } else if (!theResponse.IsSuccessStatusCode) {
                    ColorConsole.WriteError(" ERROR ");
                    Console.WriteLine($" {theResponse.ReasonPhrase}");

                    return 5;
                } else if (theResponse.ContentHeaders?.ContentType?.MediaType == "text/html") {
                    ColorConsole.WriteError(" ERROR ");
                    Console.WriteLine(" Cannot connect to host (text/html proxy found, but no json REST response).");

                    return 6;
                }

                return successAction(theResponse.Content!);
            } catch (Exception theException) {
                ColorConsole.WriteError(" ERROR ");
                Console.WriteLine($" {theException}");

                return 1;
            }
        }

        /// <summary> Executes a provided REST client and a success action, if the REST client succeeds.
        /// </summary>
        /// <remarks> Pretty-prints error messages for common HTTP status codes. </remarks>
        /// <param name="refitRestClientFunc">REST client function</param>
        /// <param name="successAction">Success action that gets the reponse from the REST client</param>
        /// <returns>Exit code</returns>
        public static int Execute(
            Func<Task<HttpResponseMessage>> refitRestClientFunc,
            Func<HttpResponseMessage, int> successAction
        ) {
            if (refitRestClientFunc == null) {
                throw new ArgumentNullException(nameof(refitRestClientFunc));
            }
            if (successAction == null) {
                throw new ArgumentNullException(nameof(successAction));
            }

            try {
                HttpResponseMessage theResponse = refitRestClientFunc().Result;

                Console.WriteLine();

                if (theResponse.StatusCode == HttpStatusCode.Unauthorized) {
                    ColorConsole.WriteError(" ERROR ");
                    Console.WriteLine(" Invalid or expired token.");

                    return 1;
                }
                if (theResponse.StatusCode == HttpStatusCode.Forbidden) {
                    ColorConsole.WriteError(" ERROR ");
                    Console.WriteLine(" You are not authorized to execute this command.");

                    return 2;
                } else if (theResponse.StatusCode == HttpStatusCode.NotFound) {
                    string theResponseMessage = theResponse.Content.ReadAsStringAsync().Result;
                    if (!string.IsNullOrEmpty(theResponseMessage)) {
                        ColorConsole.WriteError(" NOT FOUND ");
                        Console.WriteLine($" {theResponseMessage}");

                        return 3;
                    } else {
                        ColorConsole.WriteError(" ERROR ");
                        Console.WriteLine(
                            $" Not found. Invalid URL?" +
                            $" ({theResponse.RequestMessage!.Method} {theResponse.RequestMessage!.RequestUri})"
                        );

                        return 4;
                    }
                } else if (!theResponse.IsSuccessStatusCode) {
                    ColorConsole.WriteError(" ERROR ");
                    Console.WriteLine($" {theResponse.ReasonPhrase}");

                    return 5;
                } else if (theResponse.Content.Headers.ContentType?.MediaType == "text/html") {
                    ColorConsole.WriteError(" ERROR ");
                    Console.WriteLine(" Cannot connect to host (text/html proxy found, but no json REST response).");

                    return 6;
                }

                return successAction(theResponse);
            } catch (Exception theException) {
                ColorConsole.WriteError(" ERROR ");
                Console.WriteLine($" {theException}");

                return 1;
            }
        }
    }
}
