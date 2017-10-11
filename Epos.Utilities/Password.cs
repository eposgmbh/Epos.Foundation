using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Epos.Utilities
{
    public static class Password
    {
        private static readonly Aes AdvancedEncryptionStandard = CreateAdvancedEncryptionStandard();

        public static string Encrypt(string password) {
            if (password == null) {
                throw new ArgumentNullException(nameof(password));
            }

            byte[] theBytes = Encoding.UTF8.GetBytes(password);

            using (var theMemoryStream = new MemoryStream()) {
                using (var theCryptoStream = new CryptoStream(
                    theMemoryStream, AdvancedEncryptionStandard.CreateEncryptor(), CryptoStreamMode.Write
                )) {
                    theCryptoStream.Write(theBytes, 0, theBytes.Length);
                }

                return Convert.ToBase64String(theMemoryStream.ToArray());
            }
        }

        public static string Decrypt(string base64) {
            if (base64 == null) {
                throw new ArgumentNullException(nameof(base64));
            }

            var theBytes = Convert.FromBase64String(base64);

            using (var theMemoryStream = new MemoryStream()) {
                using (var theCryptoStream = new CryptoStream(
                    theMemoryStream, AdvancedEncryptionStandard.CreateDecryptor(), CryptoStreamMode.Write
                )) {
                    theCryptoStream.Write(theBytes, 0, theBytes.Length);
                }

                return Encoding.UTF8.GetString(theMemoryStream.ToArray());
            }
        }

        private static Aes CreateAdvancedEncryptionStandard() {
            Rfc2898DeriveBytes thePdb = new Rfc2898DeriveBytes(
                "BAKV2SJKSB081014",
                new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 }
            );

            Aes theAdvancedEncryptionStandard = Aes.Create();
            theAdvancedEncryptionStandard.Key = thePdb.GetBytes(32);
            theAdvancedEncryptionStandard.IV = thePdb.GetBytes(16);

            return theAdvancedEncryptionStandard;
        }
    }
}
