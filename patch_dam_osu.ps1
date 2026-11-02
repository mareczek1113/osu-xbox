# AOT: osu.Game's generic string-based transform helpers (CutoffTo, FadeAccent) pass their T to TransformTo(nameof(...)).
# Their T is unannotated, so ILC trims the property (AudioFilter.Cutoff, ...AccentColour) and TransformCustom.findAccessor fails.
# Annotate T so the members are preserved for every concrete type used at a call site.
param([string]$Root = 'D:\osu-xbox-scratch\osu-aot\osu.Game')
$ann = '[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields)] '

function Patch($path, $names) {
    $t = [IO.File]::ReadAllText($path)
    if ($t -match 'DynamicallyAccessedMembers') { "$path already patched"; return }
    foreach ($n in $names) {
        $t = $t.Replace("$n<T>(", "$n<${ann}T>(")
        $t = $t.Replace("$n<T, TEasing>(", "$n<${ann}T, TEasing>(")
    }
    $nl = if ($t.Contains("`r`n")) { "`r`n" } else { "`n" }
    $t = [regex]::Replace($t, '^(using [^\r\n]+;)', "`$1${nl}using System.Diagnostics.CodeAnalysis;", 'Multiline')
    # only keep the first inserted using (regex above inserts after every using line)
    $lines = $t -split "`r?`n"; $seen = $false; $out = foreach ($l in $lines) { if ($l -eq 'using System.Diagnostics.CodeAnalysis;') { if ($seen) { continue } else { $seen = $true } }; $l }
    [IO.File]::WriteAllText($path, ($out -join $nl))
    "patched $path"
}

Patch "$Root\Audio\Effects\ITransformableFilter.cs" @('CutoffTo')
Patch "$Root\Graphics\IHasAccentColour.cs" @('FadeAccent')
Select-String -Path "$Root\Audio\Effects\ITransformableFilter.cs", "$Root\Graphics\IHasAccentColour.cs" -Pattern 'CodeAnalysis|DynamicallyAccessedMembers' | ForEach-Object { "$($_.Filename):$($_.LineNumber): " + $_.Line.Trim().Substring(0, [Math]::Min(90, $_.Line.Trim().Length)) }
