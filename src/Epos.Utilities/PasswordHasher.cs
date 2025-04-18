using System;
using System.Security.Cryptography;

namespace Epos.Utilities;

// See https://www.youtube.com/watch?v=J4ix8Mhi3rs

/// <summary>
/// Provides functionality to hash passwords using secure algorithms.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>Hashes the specified <paramref name="password"/>.</summary>
    /// <param name="password">Password to hash</param>
    /// <returns>Hashed password</returns>
    string Hash(string password);

    /// <summary>
    /// Verifies the specified <paramref name="password"/> against the <paramref name="hashedPassword"/>.
    /// </summary>
    /// <param name="password">Password to verify</param>
    /// <param name="hashedPassword">Hashed password to compare against</param>
    /// <returns><b>true</b> if the password matches the hashed password; otherwise <b>false</b></returns>
    public bool Verify(string password, string hashedPassword);
}

/// <summary>
/// Provides functionality to hash passwords using the SHA512 algorithm.
/// </summary>
public class PasswordHasherSHA512 : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 10000;

    private static readonly HashAlgorithmName AlgorithmName = HashAlgorithmName.SHA512;

    /// <summary>Hashes the specified <paramref name="password"/>
    /// with the SHA512 Secure Hash Algorithm.</summary>
    /// <param name="password">Password to hash</param>
    /// <returns>Hashed password</returns>
    public string Hash(string password) {
        ArgumentNullException.ThrowIfNull(password);

        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, AlgorithmName, HashSize);

        return $"{Convert.ToBase64String(hash)}-{Convert.ToBase64String(salt)}";
    }

    /// <inheritdoc/>
    public bool Verify(string password, string hashedPassword) {
        ArgumentNullException.ThrowIfNull(password);
        ArgumentNullException.ThrowIfNull(hashedPassword);

        string[] parts = hashedPassword.Split('-');
        if (parts.Length != 2) {
            throw new FormatException($"Invalid hash format: {hashedPassword}");
        }

        byte[] hash = Convert.FromBase64String(parts[0]);
        byte[] salt = Convert.FromBase64String(parts[1]);

        byte[] computedHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, AlgorithmName, HashSize);

        return CryptographicOperations.FixedTimeEquals(hash, computedHash);
    }
}
