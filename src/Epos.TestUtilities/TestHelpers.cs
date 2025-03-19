using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Epos.TestUtilities;

/// <summary>
/// Provides helper methods for testing purposes.
/// </summary>
public static class TestHelpers
{
    /// <summary>
    /// Gets the path to the project folder by searching backwards for the first folder containing a .csproj file.
    /// </summary>
    public static string ProjectFolder {
        get {
            string folder = Environment.CurrentDirectory;

            while (folder is not null && Directory.GetFiles(folder, "*.csproj").Length == 0) {
                folder = Path.GetDirectoryName(folder);
            }

            return folder;
        }
    }

    /// <summary>
    /// Retrieves a stream for the specified embedded resource.
    /// </summary>
    /// <param name="resourceName">The name of the embedded resource.</param>
    /// <returns>A <see cref="Stream"/> for the specified resource.</returns>
    public static Stream GetResourceStream(string resourceName) {
        if (string.IsNullOrEmpty(resourceName)) {
            throw new ArgumentException($"'{nameof(resourceName)}' cannot be null or empty.", nameof(resourceName));
        }

        Stream stream = Assembly.GetCallingAssembly().GetManifestResourceStream(resourceName);

        if (stream is null) {
            throw new ArgumentException($"Resource '{resourceName}' not found.", nameof(resourceName));
        }

        return stream;
    }

    /// <summary>
    /// Retrieves the content of the specified embedded resource as a string.
    /// </summary>
    /// <param name="resourceName">The name of the embedded resource.</param>
    /// <returns>The content of the resource as a string.</returns>
    public static string GetResourceString(string resourceName)
    {
        if (string.IsNullOrEmpty(resourceName)) {
            throw new ArgumentException($"'{nameof(resourceName)}' cannot be null or empty.", nameof(resourceName));
        }

        using Stream stream = Assembly.GetCallingAssembly().GetManifestResourceStream(resourceName);

        if (stream is null) {
            throw new ArgumentException($"Resource '{resourceName}' not found.", nameof(resourceName));
        }

        using var streamReader = new StreamReader(stream);

        return streamReader.ReadToEnd().TrimEnd();
    }

    /// <summary>
    /// Gets a value indicating whether the current operating system is Windows.
    /// </summary>
    public static bool IsWindows => Environment.OSVersion.Platform == PlatformID.Win32NT;
}
