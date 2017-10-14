namespace Epos.CmdLine.Sample
{
    public class TestOptions
    {
        [CmdLineOption('h')]
        public bool ShowHelp { get; set; }
    }
}
