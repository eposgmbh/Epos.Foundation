namespace Epos.CommandLine;

internal sealed class CommandLineToken
{
    public CommandLineToken(CommandLineTokenKind kind, string name)
    {
        Kind = kind;
        Name = name;
    }

    public CommandLineTokenKind Kind { get; }

    public string Name { get; }

    public object? Value { get; set; }
}
