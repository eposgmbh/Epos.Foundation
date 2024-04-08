using System;

namespace Epos.TestUtilities.Docker;

/// <summary> Represents errors that occur during container start and removal. </summary>
public class DockerContainerException : Exception
{
    /// <inheritdoc />
    public DockerContainerException(string message) : base(message) { }

    /// <inheritdoc />
    public DockerContainerException(string message, Exception innerException) : base(message, innerException) { }
}
