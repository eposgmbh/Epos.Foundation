using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Epos.CodeGeneration.Sample
{
    // Important: Execute with "dotnet run" in the project folder!
    // Main assumes, your current directory is the project root folder.
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            Stream theTemplateStream =
                Assembly
                    .GetExecutingAssembly()
                    .GetManifestResourceStream("Epos.CodeGeneration.Sample.Templates.Template1.csx")!;

            await Code.Generate(
                new TemplateOptions<int> {
                    TemplateStream = theTemplateStream,
                    GeneratedSourceFilePath = Path.Combine(Environment.CurrentDirectory, "Templates", "Template1.cs"),
                    AssemblyReferences = { typeof(Program).Assembly },
                    Parameter = 99
                }
            );

            var theDocument = XDocument.Load(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("Epos.CodeGeneration.Sample.TestData.xml")
            );

            var theDrivers =
                from theDriver in theDocument.Root.Elements()
                select new Driver {
                    Name = (string) theDriver.Attribute("Name"),
                    NumberOfChampionships = (int) theDriver.Attribute("NumberOfChampionships"),
                    Team = (string) theDriver.Attribute("Team")
                };

            theTemplateStream =
                Assembly
                    .GetExecutingAssembly()
                    .GetManifestResourceStream("Epos.CodeGeneration.Sample.Templates.Template2.csx")!;

            await Code.Generate(
                new TemplateOptions<IEnumerable<Driver>> {
                    TemplateStream = theTemplateStream,
                    GeneratedSourceFilePath = Path.Combine(Environment.CurrentDirectory, "Templates", "Template2.cs"),
                    AssemblyReferences = { typeof(Program).Assembly },
                    Parameter = theDrivers
                }
            );
        }
    }
}
