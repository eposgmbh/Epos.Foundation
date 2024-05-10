using Microsoft.Extensions.Logging;

namespace Epos.CommandLine.Application;

internal sealed class LoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => Logger.Instance;

    public void Dispose() { }
}
