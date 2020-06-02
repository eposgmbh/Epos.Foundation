using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Epos.CodeGeneration
{
    /// <summary> Template context class. </summary>
    public class TemplateOptions<T>
    {
        /// <summary> Stream for the Template file (csx file). </summary>
        public Stream TemplateStream { get; set; } = null!;

        /// <summary> Path to the generated source file.</summary>
        public string GeneratedSourceFilePath { get; set; } = null!;

        /// <summary> Assembly references. </summary>
        public IList<Assembly> AssemblyReferences { get; } = new List<Assembly>();

        /// <summary> Parameter for the template. </summary>
        public T Parameter { get; set; } = default!;

        /// <summary> Write contents to the generated source file. </summary>
        /// <param name="contents"> Contents </param>
        public void Output(string contents) {
            if (contents is null) {
                throw new ArgumentNullException(nameof(contents));
            }

            if (GeneratedSourceFilePath != null) {
                File.AppendAllText(GeneratedSourceFilePath, contents);
            }
        }
    }
}
