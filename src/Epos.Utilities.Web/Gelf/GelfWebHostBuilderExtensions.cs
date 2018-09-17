using Gelf.Extensions.Logging;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Configuration
{
    /// <summary> Extension methods for the <b>IWebHostBuilder</b> type. </summary>
    public static class GelfWebHostBuilderExtensions
    {
        /// <summary> Configures GELF logging (Graylog). </summary>
        /// <param name="hostBuilder">Web host builder</param>
        /// <param name="tag">Graylog tag</param>
        /// <returns>Web host builder</returns>
        public static IWebHostBuilder ConfigureGelfLogging(this IWebHostBuilder hostBuilder, string tag = null) {
            return hostBuilder.ConfigureLogging((context, loggingBuilder) => {
                loggingBuilder.Services.Configure<GelfLoggerOptions>(context.Configuration.GetSection("Graylog"));

                if (tag != null) {
                    loggingBuilder.Services.PostConfigure<GelfLoggerOptions>(options => {
                        options.AdditionalFields["tag"] = tag;
                    });
                }

                loggingBuilder
                    .AddConfiguration(context.Configuration.GetSection("Logging"))
                    .AddConsole()
                    .AddDebug()
                    .AddGelf();
            });
        }
    }
}
