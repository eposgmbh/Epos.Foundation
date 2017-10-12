using System;
using System.IO;

namespace Epos.CmdLine
{
    public sealed class CmdLineConfiguration
    {
        private TextWriter myUsageTextWriter;
        private Action myErrorAction;

        public TextWriter UsageTextWriter {
            get => myUsageTextWriter ?? (myUsageTextWriter = Console.Out);
            set => myUsageTextWriter = value;
        }

        public Action ErrorAction {
            get => myErrorAction ?? (myErrorAction = () => { Environment.Exit(-1); });
            set => myErrorAction = value;
        }
    }
}
