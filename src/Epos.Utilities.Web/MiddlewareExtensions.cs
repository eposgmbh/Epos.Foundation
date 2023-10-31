using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Epos.Utilities.Web;

/// <summary> Collection of <b>Middleware</b> extension methods. </summary>
public static class MiddlewareExtensions
{
    /// <summary> Adds middleware for the 'X-Access-Token' cookie. Use before <b>UseAuthentication</b>. </summary>
    public static IApplicationBuilder UseJwtCookieAuthentication(this IApplicationBuilder app) {
        return app.UseMiddleware<JwtCookieAuthenticationMiddleware>();
    }

    private sealed class JwtCookieAuthenticationMiddleware
    {
        private readonly RequestDelegate myNext;

        public JwtCookieAuthenticationMiddleware(RequestDelegate next) {
            myNext = next;
        }

        public async Task Invoke(HttpContext context) {
            if (context.Request.Cookies.ContainsKey("X-Access-Token")) {
                string theToken = context.Request.Cookies["X-Access-Token"];
                context.Request.Headers["Authorization"] = $"Bearer {theToken}";
            }

            await myNext(context);
        }
    }
}
