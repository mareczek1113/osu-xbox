# Generates uwp-osu\JsonModelRoots.xml: keeps all members/constructors of osu.Game types that Newtonsoft.Json (de)serialises
# ([JsonProperty] / [JsonObject] in the type body). Without it, AOT trims their constructors ("Unable to find a constructor to use for type ...").
$root = 'D:\osu-xbox-scratch\osu-aot\osu.Game'
$typeRx = '^(\s*)(?:\[[^\]]*\]\s*)*((?:(?:public|internal|private|protected|static|sealed|abstract|partial|unsafe|readonly|file)\s+)*)(class|struct|record)\s+(\w+)(<[^>]*>)?'
$types = New-Object System.Collections.Generic.HashSet[string]
$skipped = 0
foreach ($f in (Get-ChildItem $root -Recurse -Filter *.cs | Where-Object { $_.FullName -notmatch '\\(obj|bin|Tests)\\' })) {
    $t = [IO.File]::ReadAllText($f.FullName)
    if ($t -notmatch '\[Json(Property|Object|Constructor)') { continue }
    $lines = $t -split "`r?`n"
    $ns = [regex]::Match($t, '(?m)^\s*namespace\s+([\w\.]+)').Groups[1].Value
    if (-not $ns) { continue }
    for ($i = 0; $i -lt $lines.Count; $i++) {
        $m = [regex]::Match($lines[$i], $typeRx)
        if (-not $m.Success) { continue }
        $indent = $m.Groups[1].Value.Length
        if ($m.Groups[2].Value -match '\bstatic\b|\bprivate\b') { continue }
        # body = following lines until a closing brace at the same indent
        $j = $i + 1; $body = $false
        while ($j -lt $lines.Count) {
            $l = $lines[$j]
            if ($l -match '^\s{0,' + $indent + '}\}') { break }
            if ($l -match '\[Json(Property|Object|Constructor)') { $body = $true }
            $j++
        }
        # the [JsonObject] attribute may sit on the line(s) above the declaration
        if (-not $body -and $i -gt 0 -and $lines[$i - 1] -match '\[Json(Object|Converter)') { $body = $true }
        if (-not $body) { continue }
        # enclosing types
        $chain = @(); $cur = $indent
        for ($k = $i - 1; $k -ge 0 -and $cur -gt 0; $k--) {
            $pm = [regex]::Match($lines[$k], $typeRx)
            if ($pm.Success -and $pm.Groups[1].Value.Length -lt $cur) {
                $nm = $pm.Groups[4].Value; if ($pm.Groups[5].Success -and $pm.Groups[5].Value) { $nm += '`' + (($pm.Groups[5].Value -split ',').Count) }
                $chain = , $nm + $chain; $cur = $pm.Groups[1].Value.Length
            }
        }
        $name = $m.Groups[4].Value; if ($m.Groups[5].Success -and $m.Groups[5].Value) { $name += '`' + (($m.Groups[5].Value -split ',').Count) }
        [void]$types.Add("$ns." + (($chain + $name) -join '/'))
    }
}
$xml = "<linker>`n  <assembly fullname=`"osu.Game`">`n" + (($types | Sort-Object | ForEach-Object { "    <type fullname=`"$_`" preserve=`"all`" />" }) -join "`n") + "`n  </assembly>`n</linker>`n"
Set-Content -Path D:\osu-xbox-scratch\uwp-osu\JsonModelRoots.xml -Value $xml -Encoding UTF8
"types: $($types.Count)"
"APISeasonalBackgrounds: $($types.Contains('osu.Game.Online.API.Requests.Responses.APISeasonalBackgrounds'))  APIMenuContent: $($types.Contains('osu.Game.Online.API.Requests.Responses.APIMenuContent'))"
$types | Where-Object { $_ -match '/' } | Select-Object -First 5
