namespace Epos.CmdLine
{
    internal sealed class CmdLineToken
    {
        public CmdLineToken(CmdLineTokenKind kind, string name) {
            Kind = kind;
            Name = name;
        }

        public CmdLineTokenKind Kind { get; }

        public string Name { get; }

        public object Value { get; set; }
    }
}
