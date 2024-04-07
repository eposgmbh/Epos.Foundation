using System;
using System.Security.Cryptography;
using System.Text;

namespace Epos.Utilities
{
    /// <summary>Helper for hashing passwords (for example
    /// to store passwords in a database).</summary>
    public static class Password
    {
        /// <summary>Hashes the specified <paramref name="password"/>
        /// with the SHA256 Secure Hash Algorithm.</summary>
        /// <param name="password">Password to hash</param>
        /// <returns>Hashed password</returns>
        public static string Hash(string password) {
            if (password is null) {
                throw new ArgumentNullException(nameof(password));
            }

            using (var theSha256 = SHA256.Create()) {
                byte[] thePasswordBytes = Encoding.UTF8.GetBytes(password);
                byte[] theHashedBytes = theSha256.ComputeHash(thePasswordBytes);

                return BitConverter.ToString(theHashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}
