using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Logging;

using NUnit.Framework;

namespace Epos.Utilities.Web;

[TestFixture]
public class HostExtensionsTest
{
    [Test]
    public void Url() {
        var theLogger = new Logger();

        var theUrl = "https://blog.eposgmbh.eu";

        HostExtensions.WaitForServiceAvailability(theUrl, 10, true, theLogger);

        Assert.That(theLogger.LogMessages, Has.One.StartsWith("Information:Host blog.eposgmbh.eu:443 is available after"));
    }

    [Test]
    public void ConnectionString() {
        var theConnectionString = "Server=unknown.server.com;Port=5432;Database=db;User ID=admin;Password=admin";

        try {
            HostExtensions.WaitForServiceAvailability(theConnectionString, 2, true, null!);
        } catch (TimeoutException theException) {
            Assert.That(theException.Message, Does.StartWith("Host unknown.server.com:5432 is not available after 2 seconds."));
        }
    }
}

public class Logger : ILogger
{
    public IDisposable BeginScope<TState>(TState state) { return null!; }

    public bool IsEnabled(LogLevel logLevel) { return true; }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) {
        var theFormattedLogValues = (IReadOnlyList<KeyValuePair<string, object?>>?) state;
        LogMessages.Add(logLevel + ":" + theFormattedLogValues!.First().Value);
    }

    public List<string> LogMessages {get;} = new List<string>();
}
