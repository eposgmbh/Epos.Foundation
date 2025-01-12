using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Epos.Utilities.Web;

/// <summary> Collection of <b>DbContext</b> extension methods. </summary>
public static class DbContextExtensions
{
    /// <summary> Waits for the availability of the underlying database of the supplied data context.
    /// After the provided timeout an exception is thrown. </summary>
    /// <param name="dbContext">Database context</param>
    /// <param name="timeoutSeconds">Timeout in seconds</param>
    /// <returns> Database context </returns>
    /// <exception cref="TimeoutException">Thrown when the timeout has run out.</exception>
    public static DbContext WaitForDatabaseAvailability(this DbContext dbContext, int timeoutSeconds) {
        if (dbContext is null) {
            throw new ArgumentNullException(nameof(dbContext));
        }

        IInfrastructure<IServiceProvider> theInfrastructure = dbContext;
        ILogger theLogger = theInfrastructure.GetService<ILogger<Logging>>();

        string theConnectionString = dbContext.Database.GetConnectionString() ??
            throw new InvalidOperationException($"Invalid connection string on {dbContext.GetType()}");

        HostExtensions.WaitForServiceAvailability(theConnectionString, theLogger, timeoutSeconds);

        return dbContext;
    }
}
