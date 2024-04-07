# Epos.Foundation

![Build Status](https://eposgmbh.visualstudio.com/_apis/public/build/definitions/30ebff28-f13c-44d2-b6db-7739d6cf4ab1/5/badge)
[![NuGet](https://img.shields.io/nuget/v/Epos.Utilities.svg)](https://www.nuget.org/packages/Epos.Utilities/)
![License](https://img.shields.io/badge/license-MIT-blue.svg)
![Downloads](https://img.shields.io/nuget/dt/Epos.Utilities.svg)

Build and Release deployment (Docs Website and [NuGet](https://www.nuget.org/)) is automated with
[Azure DevOps](https://dev.azure.com).

## Docs

Epos.Foundation is the Github Repo for foundational utilities like String or Dictionary extension methods
(NuGet package **Epos.Utilities**), utilities for web apps and web APIs (NuGet package **Epos.Utilities.Web**) and a powerful and simple command line parser (NuGet package **Epos.CommandLine**).

The packages are implemented using .NET 8+. Therefore you can use them cross-platform on any supported platform.

[See the Docs Website.](https://eposgmbh.github.io/getting-started.html)

## Installation

Via NuGet you can install the following NuGet packages:

```bash
$ dotnet add package Epos.Utilities
$ dotnet add package Epos.Utilities.Web
$ dotnet add package Epos.CommandLine
$ dotnet add package Epos.TestUtilities
```

You can install them separately, the packages are independent from each other.

## Usage

**Epos.Utilities** and **Epos.Utilities.Web** are more or less self-documenting simple utility classes. You can take a
look at the corresponding unit tests in [this Github Repo](https://github.com/eposgmbh/Epos.Foundation/tree/master/src/Epos.Utilities.Tests).

**Epos.TestUtilities** allows spinning up Docker containers for integration or unit test runs. See the corresponging unit tests in [this Github Repo](https://github.com/eposgmbh/Epos.Foundation/tree/master/src/Epos.TestUtilities.Tests).

**Epos.CommandLine** is a full fledged yet simple command line parser. Source code for a demo console app can be found in
[this Github Repo](https://github.com/eposgmbh/Epos.Foundation/tree/master/src/Epos.CommandLine.Sample).

### Epos.Utilities

Highlights of the **Epos.Utilities** package are:

#### Arithmetics

Use the `Arithmetics` class to do generic calculations for different data types like
`int` or `double`:

```csharp
double theDouble = Arithmetics.GetZeroValue<double>(); // 0.0
int theInteger = Arithmetics.GetOneValue<int>(); // 1

var theAddIntegers = Arithmetics.CreateAddOperation<int>();
int theSum = theAddIntegers(theInteger, 33); // 34

var theMultiplyDoubles = Arithmetics.CreateMultiplyOperation<double>();
double theProduct = theMultiplyDoubles(11.0, 6.5); // 71.5
```

#### StringExtensions

Use the `StringExtensions` class to do (among other things) generic type conversions
from a string. You can convert to any type with an attached `TypeConverterAttribute`.

```csharp
bool isSuccess = "33".TryConvert(CultureInfo.InvariantCulture, out int theInteger);
Assert.That(theInteger, Is.EqualTo(33));
Assert.That(isSuccess, Is.True);

// or equivalent:

isSuccess = "33".TryConvert(typeof(int), CultureInfo.InvariantCulture, out object theObject);
Assert.That(theObject, Is.EqualTo(33));
Assert.That(isSuccess, Is.True);

// Failure case:

isSuccess = "ABC".TryConvert(CultureInfo.InvariantCulture, out double theDouble);
Assert.That(theDouble, Is.EqualTo(0.0));
Assert.That(isSuccess, Is.False);
```

#### DumpExtensions

Use the `DumpExtensions` class to pretty-print any kind of object.

```csharp
// null values
object obj = null;
Console.WriteLine(obj.Dump()); // Null

// Types
Console.WriteLine(typeof(int).Dump()); // int
Console.WriteLine(typeof(double).Dump()); // double
Console.WriteLine(typeof(double?).Dump()); // double?
Console.WriteLine(typeof(Cache<,>).Dump()); // Epos.Utilities.Cache<TKey, TValue>

double dbl = 33.99;
Console.WriteLine(dbl.Dump()); // 33.99 (Dump always uses the invariant culture)

// Complex type instances:
Console.WriteLine(new[] { 1, 2, 3 }.Dump()); // [1, 2, 3]
Console.WriteLine(new { Integer = 1, String = "Hello" }.Dump()); // { Integer = 1, String = Hello }
```

#### Container

Use the `Container` class as a simple lightweight DI Container.

```csharp
var theContainer = new Container();
theContainer
    .Register<ITestService>()
    .ImplementedBy<TestService>()
    .WithParameter("connectionString", "Hello World!") // Constructor params
    .AndParameter("maxCount", 10)
    .WithLifetime(Lifetime.Singleton);

// ...

var theTestService = theContainer.Resolve<ITestService>();
```

### Epos.CommandLine

Sample `BuildOptions` class for strongly typed access of the command line parameters:

```csharp
public class BuildOptions
{
    [CommandLineOption('p')]
    public int ProjectNumber { get; set; }

    [CommandLineOption('m')]
    public string Memory { get; set; }

    // ...
}
```

Sample entry point that uses the `BuildOptions` class for the subcommand `build`:

```csharp
var theCommandLineDefinition = new CommandLineDefinition {
    Name = "sample", // <- if not specified, .exe-Filename is used
    Subcommands = {
        new CommandLineSubcommand<BuildOptions>("build", "Builds something.") {
            Options = {
                new CommandLineOption<int>('p', "Sets the project number.") { LongName = "project-number" },
                new CommandLineOption<string>('m', "Sets the used memory.") {
                    LongName = "memory",
                    DefaultValue = "1 GB"
                },
                new CommandLineSwitch('d', "Disables the command."),
                new CommandLineSwitch('z', "Zzzz...")
            },
            Parameters = {
                new CommandLineParameter<string>("filename", "Sets the filename.")
            },
            CommandLineFunc = (options, definition) => {
                // Do something for the build subcommand
                // ...

                Console.WriteLine("sample command line application" + Lf);
                Console.WriteLine(options.Dump() + Lf);

                return 0; // <- your error code or 0, if successful
            }
        }
    },
    // further subcommands...
    //
    // If you don't want to specify subcommands, register one subcommand with the name "default"
    // (or use the constant CommandLineSubcommand.DefaultName) and set the HasDifferentiatedSubcommands
    // property of the CommandLineDefinition to false.
};

return theCommandLineDefinition.Try(args);
```

This simple definition automatically produces help and usage console output like this:

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

Long option chains and description texts are properly line breaked regarding the console window width.

## Contributing

You can enter an issue on Github [here](https://github.com/eposgmbh/Epos.Foundation/issues). I will work on
your issue when I have time. Pull Requests are possible too.

## License

MIT License

Copyright (c) 2017-2024 eposgmbh

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
