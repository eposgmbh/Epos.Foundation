Push-Location $(Split-Path -Parent $PSScriptRoot)

dotnet test src/Epos.CommandLine.Tests   --verbosity quiet
dotnet test src/Epos.Utilities.Tests     --verbosity quiet
dotnet test src/Epos.TestUtilities.Tests --verbosity quiet
dotnet test src/Epos.Utilities.Web.Tests --verbosity quiet

Pop-Location
