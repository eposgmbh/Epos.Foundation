namespace Epos.CmdLine.Sample
{
    public class BuildOptions
    {
        [CmdLineOption('p')]
        public int ProjectNumber { get; set; }

        [CmdLineOption('m')]
        public string Memory { get; set; }

        [CmdLineOption('d')]
        public bool Disabled { get; set; }

        [CmdLineOption('z')]
        public bool Zzzz { get; set; }

        [CmdLineParameter("Filename")]
        public string File { get; set; }
    }
}
