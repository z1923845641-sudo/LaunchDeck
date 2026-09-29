$ErrorActionPreference='Stop'
$project=Join-Path $PSScriptRoot 'src\LaunchDeck\LaunchDeck.csproj'
$msbuild=(Get-Command msbuild -ErrorAction SilentlyContinue).Source
if(-not $msbuild){throw 'Visual Studio Build Tools 2022 / MSBuild is required.'}
& $msbuild $project /p:Configuration=Release /restore
if($LASTEXITCODE -ne 0){exit $LASTEXITCODE}
