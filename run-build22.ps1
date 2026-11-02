$sw = [Diagnostics.Stopwatch]::StartNew()
& D:\osu-xbox-scratch\package-uwp.ps1 -Project uwp-osu -Name UwpOsu -Install -Extra @('-p:XboxPort=true') *> D:\osu-xbox-scratch\build22.out.txt
"exit $LASTEXITCODE after $([int]$sw.Elapsed.TotalSeconds)s" | Out-File D:\osu-xbox-scratch\build22.done
