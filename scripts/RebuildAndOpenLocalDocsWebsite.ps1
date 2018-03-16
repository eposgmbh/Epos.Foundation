Push-Location $(Split-Path -Parent $PSScriptRoot)

Remove-Item ./docs/bin -Force -Recurse -Confirm:$false
Remove-Item ./docs/obj -Force -Recurse -Confirm:$false

dotnet build ./docs

./scripts/OpenLocalDocsWebsite.ps1

Pop-Location
