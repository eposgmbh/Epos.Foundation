using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;
using NUnit.Framework.Internal;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace Epos.CommandLine.Application;

[TestFixture]
public class CommandLineApplicationBuilderTest
{
    [Test]
    public async Task Basics() {
        HostApplicationBuilder theBuilder = CommandLineApplicationBuilder.CreateApplicationBuilder(["--test", "World"]);

        theBuilder.Configure(x => {
            x.Name = "application-builder";
        });

        theBuilder.Services.AddSingleton<TestService>();
        theBuilder.Services.AddSubcommand<MySubcommand>();

        await theBuilder
            .Build()
            .RunAsync();

        Assert.That(Environment.ExitCode, Is.EqualTo(123));
    }

    internal class TestService
    {
        public string GetSomeString() => "Hello, World!";
    }

    internal class MySubcommand(TestService testService, ILogger<MySubcommand> logger) : ISubcommand
    {
        public string Description => "Default command";

        [Option('t', "Test option", LongName = "test")]
        public string? TestOption { get; set; }

        [Parameter("test-param", "Test parameter", DefaultValue = 123)]
        public int TestParam { get; set; }


        public Task<int> ExecuteAsync(CancellationToken cancellationToken) {
            if (TestOption == "World" && testService.GetSomeString().StartsWith("Hello")) {
                logger.LogInformation("Successful!");
                return Task.FromResult(TestParam);
            } else {
                logger.LogError("Unsuccessful!");
                return Task.FromResult(0);
            }
        }
    }
}
