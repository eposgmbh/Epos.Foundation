namespace Epos.CommandLine.Sample
{
    public class TestOptions
    {
        [CommandLineOption('h')]
        public bool ShowHelp { get; set; }
    }
}
