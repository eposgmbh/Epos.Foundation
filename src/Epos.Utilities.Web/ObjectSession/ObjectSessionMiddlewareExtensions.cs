using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

using Epos.Utilities.Web;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary> Provides extensions methods for the <see cref="ObjectSession" />
/// class. </summary>
public static class ObjectSessionMiddlewareExtensions
{
    /// <summary> Adds the <see cref="ObjectSession" /> middleware.
    /// </summary>
    /// <param name="app">Application builder</param>
    /// <returns>Application builder</returns>
    public static IApplicationBuilder UseObjectSession(this IApplicationBuilder app) {
        return app.UseSession();
    }
}
