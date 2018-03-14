using System;
using System.IO;

namespace Epos.CmdLine
{
    /// <summary>Configuration class for the
    /// <see cref="CmdLineDefinition"/>.</summary>
    public sealed class CmdLineConfiguration
    {
        private TextWriter myUsageTextWriter;
        private Action myErrorAction;

        /// <summary>Specifies the <see cref="System.IO.TextWriter"/> that is used
        /// to write usage help.</summary>
        /// <remarks>Defaults to <see cref="System.Console.Out"/>.</remarks>
        public TextWriter UsageTextWriter {
            get => myUsageTextWriter ?? (myUsageTextWriter = Console.Out);
            set => myUsageTextWriter = value;
        }

        /// <summary>Specifies the action that is taken when the command line
        /// cannot be parsed successfully.</summary>
        /// <remarks>Defaults to:
        /// <code>Environment.Exit(-1);</code>
        /// </remarks>
        public Action ErrorAction {
            get => myErrorAction ?? (myErrorAction = () => { Environment.Exit(-1); });
            set => myErrorAction = value;
        }
    }
}
