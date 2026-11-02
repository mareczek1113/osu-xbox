param([Parameter(Mandatory)][string]$Project, [Parameter(Mandatory)][string]$Name, [switch]$Install, [string[]]$Extra = @())
# Usage: package-uwp.ps1 -Project uwp-render -Name UwpRender [-Install]
$ErrorActionPreference = 'Stop'
$root = "D:\osu-xbox-scratch\$Project"
$msb  = 'D:\VS2026BuildTools\MSBuild\Current\Bin\amd64\MSBuild.exe'
$kit  = 'C:\Program Files (x86)\Windows Kits\10\bin\10.0.26100.0\x64'
$thumb = 'A728712F9857223E41FFD29C467F6CFBC52332CA'
$bin  = "$root\bin\x64\Release\net10.0-windows10.0.26100.0\win-x64"
$lay  = "$root\layout"
$out  = "$root\out"

Push-Location $root
Remove-Item "$bin\publish\$Name.exe" -ErrorAction SilentlyContinue   # a failed publish must not repackage a stale exe
& $msb "$Name.csproj" -restore -t:Publish -p:Configuration=Release -p:Platform=x64 @Extra -v:m -nologo 2>&1 | Select-String -Pattern 'error|warning IL|Generating|->' | Select-Object -Last 15
if (-not (Test-Path "$bin\publish\$Name.exe")) { throw "Publish produced no exe" }
Pop-Location

Remove-Item $lay, $out -Recurse -Force -ErrorAction SilentlyContinue
New-Item -ItemType Directory $lay, $out | Out-Null
Copy-Item "$bin\publish\*" $lay -Recurse -Exclude *.pdb
Copy-Item "$bin\*.xbf" $lay
Copy-Item "$bin\$Name.pri" "$lay\resources.pri"
Copy-Item "$root\Assets" "$lay\Assets" -Recurse
$ver = '1.0.' + [int]((Get-Date) - [datetime]'2026-10-30').TotalMinutes + '.0'; (Get-Content "$root\Package.appxmanifest" -Raw).Replace('$targetnametoken$', $Name).Replace('x-generate', 'en-US').Replace('Version="1.0.0.0"', "Version=`"$ver`"") | Set-Content "$lay\AppxManifest.xml" -Encoding UTF8

& "$kit\makeappx.exe" pack /d $lay /p "$out\$Name.appx" /o | Select-Object -Last 1
& "$kit\signtool.exe" sign /fd SHA256 /sha1 $thumb "$out\$Name.appx" | Out-Null
Copy-Item "D:\osu-xbox-scratch\uwp-hello\out\OsuXboxDev.cer" $out -ErrorAction SilentlyContinue
if ($Install) { Add-AppxPackage -Path "$out\$Name.appx" -ForceUpdateFromAnyVersion; "installed" }
Get-ChildItem $out | Select-Object Name, Length | Format-Table | Out-String

