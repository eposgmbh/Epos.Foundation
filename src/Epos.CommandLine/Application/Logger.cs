using System;

using Microsoft.Extensions.Logging;

using Epos.Utilities;

namespace Epos.CommandLine.Application;

internal sealed class Logger : ILogger
{
    public static readonly ILogger Instance = new Logger();

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;


    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
        Exception? exception, Func<TState, Exception?, string> formatter) {
        if (!IsEnabled(logLevel)) {
            return;
        }

        switch (logLevel) {
            case LogLevel.Warning:
                ColorConsole.WriteWarning(" WARNING ");
                Console.Write(' ');
                break;
            case LogLevel.Error:
                ColorConsole.WriteError(" ERROR ");
                Console.Write(' ');
                break;
            case LogLevel.Critical:
                ColorConsole.WriteError(" CRITICAL ");
                Console.Write(' ');
                break;
            case LogLevel.Trace:
                ColorConsole.WriteHint(" INFO ");
                Console.Write(' ');
                break;
            case LogLevel.Debug:
                ColorConsole.WriteHint(" DEBUG ");
                Console.Write(' ');
                break;
            default:
                // Information: Do nothing
                break;
        }

        Console.WriteLine(formatter(state, exception));
    }
}
