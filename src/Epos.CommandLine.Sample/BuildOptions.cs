namespace Epos.CommandLine.Sample;

public class BuildOptions
{
    [CommandLineOption('p')]
    public int ProjectNumber { get; set; }

    [CommandLineOption('m')]
    public string? Memory { get; set; }

    [CommandLineOption('d')]
    public bool Disabled { get; set; }

    [CommandLineOption('z')]
    public bool Zzzz { get; set; }

    [CommandLineParameter("Filename")]
    public string? File { get; set; }
}
