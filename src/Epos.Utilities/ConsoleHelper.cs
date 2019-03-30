using System;
using System.Text;

namespace Epos.Utilities
{
    /// <summary> Provides Console helper methonds. </summary>
    public static class ConsoleHelper
    {
        /// <summary> Reads a password from Standard Input and displays an asterisk for each character.
        /// </summary>
        /// <returns>Password</returns>
        public static string ReadPassword() {
            var thePassword = new StringBuilder();

            ConsoleKeyInfo theNextKey = Console.ReadKey(intercept: true);
            while (theNextKey.Key != ConsoleKey.Enter) {
                if (theNextKey.Key == ConsoleKey.Backspace) {
                    if (thePassword.Length > 0) {
                        thePassword.Remove(thePassword.Length - 1, 1);

                        Console.Write(theNextKey.KeyChar);
                        Console.Write(" ");
                        Console.Write(theNextKey.KeyChar);
                    }
                } else {
                    thePassword.Append(theNextKey.KeyChar);
                    Console.Write("*");
                }

                theNextKey = Console.ReadKey(true);
            }

            Console.WriteLine();

            return thePassword.ToString();
        }
    }
}
