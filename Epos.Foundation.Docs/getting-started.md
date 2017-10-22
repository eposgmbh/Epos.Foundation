# Getting started

This is the Epos.Foundation Docs Website.

## Description

Epos.Foundation is the Github Repo for foundational utilities like String or Dictionary extension methods
(NuGet package **Epos.Utilities**) and a powerful and simple command line parser (NuGet package **Epos.CmdLine**).

The packages are implemented using .NET Standard (2.0+). Therefore you can use them cross-platform on any supported platform and
also with the full .NET Framework (4.6.1+).

## Installation

Via NuGet you can install the NuGet packages **Epos.Utilities** and **Epos.CmdLine**.

```
PM> Install-Package Epos.Utilities
PM> Install-Package Epos.CmdLine
```

You can install them separately, the packages are independent from each other.

## Usage

**Epos.Utilities** are more or less self-documenting simple utility classes. You can take a look at the corresponding
unit tests in [this Github Repo](https://github.com/eposgmbh/Epos.Foundation/tree/master/Epos.Utilities.Tests).

**Epos.CmdLine** is a full fledged yet simple command line parser. Source code for a demo console app can be found in
[this Github Repo](https://github.com/eposgmbh/Epos.Foundation/tree/master/Epos.CmdLine.Sample). It's as simple as
this:

```csharp
public static int Main(string[] args) {
    var theCmdLineDefinition = new CmdLineDefinition {
        Name = "sample", // <- if null, .exe-Filename is taken
        Subcommands = {
            new CmdLineSubcommand<BuildOptions>("build", "Builds something.") {
                Options = {
                    new CmdLineOption<int>('p', "Sets the project number.") { LongName = "project-number" },
                    new CmdLineOption<string>('m', "Sets the used memory.") {
                        LongName = "memory",
                        DefaultValue = "1 GB"
                    },
                    new CmdLineSwitch('d', "Disables the command."),
                    new CmdLineSwitch('z', "Zzzz...")
                },
                Parameters = {
                    new CmdLineParameter<string>("filename", "Sets the filename.")
                },
                CmdLineFunc = (options, definition) => {
                    // Do something for the build subcommand
                    // ...

                    Console.WriteLine("sample command line application" + Lf);
                    Console.WriteLine(options.Dump() + Lf);

                    return 0; // <- your error code or 0, if successful
                }
            }
        },
        // further subcommands...
    };

    return theCmdLineDefinition.Try(args);
}
```

This definition automatically produces help and usage console output like this:

```
Usage: sample build [-p, --project-number <int>] [-m, --memory <string="1 GB">] [-d] [-z] <filename:string>

Error: Missing parameter: filename

Options
  -p, --project-number   Sets the project number.
  -m, --memory           Sets the used memory. >>> defaults to "1 GB"
  -d                     Disables the command.
  -z                     Zzzz...

Parameters
  filename               Sets the filename.
```

Long option chains and description texts are properly line breaked.
