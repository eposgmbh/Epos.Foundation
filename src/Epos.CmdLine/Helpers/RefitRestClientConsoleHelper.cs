using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Epos.Utilities;

using Refit;

namespace Epos.CmdLine.Helpers;

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
        Func<Task<T>> refitRestClientFunc,
        Func<T, int> successAction
    ) {
        if (refitRestClientFunc == null) {
            throw new ArgumentNullException(nameof(refitRestClientFunc));
        }
        if (successAction == null) {
            throw new ArgumentNullException(nameof(successAction));
        }

        try {
            T theResponse = refitRestClientFunc().Result;

            Console.WriteLine();

            return successAction(theResponse);
        } catch (AggregateException theException) {
            Exception theInnerException = theException.InnerExceptions.First();
            
            if (theInnerException is ApiException theApiException) {
                return HandleApiException(theApiException);
            } else {
                return HandleException(theInnerException);
            }
        } catch (ApiException theException) {
            return HandleApiException(theException);
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
    public static int Execute(
        Func<Task> refitRestClientFunc,
        Func<int> successAction
    ) {
        if (refitRestClientFunc == null) {
            throw new ArgumentNullException(nameof(refitRestClientFunc));
        }
        if (successAction == null) {
            throw new ArgumentNullException(nameof(successAction));
        }

        try {
            refitRestClientFunc().Wait();

            Console.WriteLine();

            return successAction();
        } catch (AggregateException theException) {
            Exception theInnerException = theException.InnerExceptions.First();
            
            if (theInnerException is ApiException theApiException) {
                return HandleApiException(theApiException);
            } else {
                return HandleException(theInnerException);
            }
        } catch (ApiException theException) {
            return HandleApiException(theException);
        } catch (Exception theException) {
            return HandleException(theException);
        }
    }

    #region --- Helper methods ---

    private static int HandleApiException(ApiException theApiException) {
        Console.WriteLine();

        if (theApiException.StatusCode == HttpStatusCode.Unauthorized) {
            ColorConsole.WriteError(" ERROR ");
            Console.WriteLine(" Invalid or expired token.");

            return 1;
        } else if (theApiException.StatusCode == HttpStatusCode.Forbidden) {
            ColorConsole.WriteError(" ERROR ");
            Console.WriteLine(" You are not authorized to execute this command.");

            return 2;
        } else if (theApiException.StatusCode == HttpStatusCode.NotFound) {
            ColorConsole.WriteError(" ERROR ");
            Console.WriteLine(
                $" Not found. Invalid URL?" +
                $" ({theApiException.RequestMessage.Method} {theApiException.RequestMessage.RequestUri})"
            );

            return 4;
        } else if (theApiException.ContentHeaders?.ContentType.MediaType == "text/html") {
            ColorConsole.WriteError(" ERROR ");
            Console.WriteLine(" Cannot connect to host (text/html proxy found, but no json REST response).");

            return 5;
        } else {
            ColorConsole.WriteError(" ERROR ");
            Console.WriteLine($" {theApiException.Content}");

            return 6;
        }
    }

    private static int HandleException(Exception theException) {
        Console.WriteLine();
        ColorConsole.WriteError(" ERROR ");
        Console.WriteLine($" {theException.Message}");

        return 999;
    }

    #endregion
}
