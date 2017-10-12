namespace Epos.CmdLine
{
    public sealed class CmdLineSwitch : CmdLineOption<bool>
    {
        public CmdLineSwitch(char letter, string description) : base(letter, description) { }
    }
}
