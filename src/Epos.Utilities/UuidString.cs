using System;

namespace Epos.Utilities;

/// <summary> Helpers for UUIDs. </summary>
public static class UuidString
{
    /// <summary> Creates the string of a new UUID in the format 'abcd1234abcd1234abcd1234abcd1234'.
    /// </summary>
    /// <returns>UUID string</returns>
    public static string Create() => Guid.NewGuid().ToString("N").ToLowerInvariant();
}
