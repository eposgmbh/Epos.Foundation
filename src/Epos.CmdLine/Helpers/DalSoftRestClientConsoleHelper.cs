using System;
using System.Net;

using Epos.Utilities;

namespace Epos.CmdLine.Helpers
{
    /// <summary> Provides helper methods for the DalSoft dynamic REST Client. </summary>
    public static class DalSoftRestClientConsoleHelper
    {
        /// <summary> Executes a provided REST client and a success action, if the REST client succeeds.
        /// </summary>
        /// <remarks> Pretty-prints error messages for common HTTP status codes. </remarks>
        /// <param name="dalSoftRestClientFunc">REST client function</param>
        /// <param name="successAction">Success action that gets the reponse from the REST client</param>
        /// <returns>Exit code</returns>
        public static int Execute(Func<dynamic> dalSoftRestClientFunc, Func<dynamic, int> successAction) {
            if (dalSoftRestClientFunc == null) {
                throw new ArgumentNullException(nameof(dalSoftRestClientFunc));
            }
            if (successAction == null) {
                throw new ArgumentNullException(nameof(successAction));
            }

            try {
                dynamic theResponse = dalSoftRestClientFunc();

                if (theResponse.HttpResponseMessage.StatusCode == HttpStatusCode.Unauthorized) {
                    Console.WriteLine();

                    ColorConsole.WriteError(" ERROR ");
                    Console.WriteLine(" Invalid or expired token.");

                    return 1;
                }
                if (theResponse.HttpResponseMessage.StatusCode == HttpStatusCode.Forbidden) {
                    Console.WriteLine();

                    ColorConsole.WriteError(" ERROR ");
                    Console.WriteLine(" You are not authorized to execute this command.");

                    return 1;
                } else if (!theResponse.HttpResponseMessage.IsSuccessStatusCode) {
                    Console.WriteLine();

                    ColorConsole.WriteError(" ERROR ");
                    Console.WriteLine($" {theResponse.ToString()}");

                    return 1;
                }

                Console.WriteLine();

                return successAction(theResponse);
            } catch (Exception theException) {
                Console.WriteLine();

                ColorConsole.WriteError(" ERROR ");
                Console.WriteLine($" {theException.Message}");

                return 1;
            }
        }
    }
}
