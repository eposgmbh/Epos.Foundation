using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Epos.CodeGeneration
{
    /// <summary> Generates code from a csx-Template and a parameter (used in that template).
    /// </summary>
    public static class Code
    {
        /// <summary> Generated code from the specified template and parameter.
        /// </summary>
        /// <param name="options"> Template options </param>
        /// <typeparam name="T"> Type of the parameter </typeparam>
        /// <returns>Task</returns>
        public static async Task Generate<T>(TemplateOptions<T> options) {
            if (options.TemplateStream == null) {
                throw new ArgumentNullException(nameof(TemplateOptions<T>.TemplateStream));
            }
            if (options.GeneratedSourceFilePath == null) {
                throw new ArgumentNullException(nameof(TemplateOptions<T>.GeneratedSourceFilePath));
            }
            if (options.Parameter == null) {
                throw new ArgumentNullException(nameof(TemplateOptions<T>.Parameter));
            }

            File.Delete(options.GeneratedSourceFilePath);

            var theScriptOptions =
                ScriptOptions
                    .Default
                    .WithReferences(typeof(Code).Assembly)
                    .WithReferences(options.AssemblyReferences)
                    .WithImports("System", "Epos.CodeGeneration");

            var theScript = CSharpScript.Create(options.TemplateStream, theScriptOptions, typeof(TemplateOptions<T>));

            options.TemplateStream.Close();

            await theScript.RunAsync(options);
        }        
    }
}