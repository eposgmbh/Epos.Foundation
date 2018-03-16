Push-Location $(Split-Path -Parent $PSScriptRoot)

Start-Process docs/bin/Website/getting-started.html

Pop-Location
