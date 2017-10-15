# Epos.Foundation

![Build Status](https://eposgmbh.visualstudio.com/_apis/public/build/definitions/30ebff28-f13c-44d2-b6db-7739d6cf4ab1/5/badge)

(Build and Release deployment to [NuGet](https://www.nuget.org/) is automated with
[Visual Studio Team Services](https://www.visualstudio.com/team-services).

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

## Contributing

You can enter an issue on Github [here](https://github.com/eposgmbh/Epos.Foundation/issues). I will work on
your issue when I have time. Pull Requests are possible too.

## License

MIT License

Copyright (c) 2017 eposgmbh

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
