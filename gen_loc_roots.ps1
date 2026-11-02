# Generates uwp-osu\LocalisationRoots.xml: keeps all members of osu.Game's *Strings localisation classes.
# They are looked up by name via LocalisableDescriptionAttribute(typeof(X), nameof(X.Member)) at runtime.
$root = 'D:\osu-xbox-scratch\osu-aot\osu.Game'
$types = New-Object System.Collections.Generic.HashSet[string]
Get-ChildItem $root -Recurse -Filter *.cs | Where-Object { $_.FullName -notmatch '\\(obj|bin|Tests)\\' } | ForEach-Object {
  $t = [IO.File]::ReadAllText($_.FullName)
  if ($t -notmatch 'LocalisableString|LocalisableDescription|TranslatableString') { return }
  $ns = [regex]::Match($t, '(?m)^namespace\s+([\w\.]+)').Groups[1].Value
  foreach ($m in [regex]::Matches($t, '(?m)^\s*public\s+(?:static\s+|partial\s+)*class\s+(\w+Strings)\b')) { [void]$types.Add("$ns.$($m.Groups[1].Value)") }
}
$xml = "<linker>`n  <assembly fullname=`"osu.Game`">`n" + (($types | Sort-Object | ForEach-Object { "    <type fullname=`"$_`" preserve=`"all`" />" }) -join "`n") + "`n  </assembly>`n</linker>`n"
Set-Content -Path D:\osu-xbox-scratch\uwp-osu\LocalisationRoots.xml -Value $xml -Encoding UTF8
"types: $($types.Count)"
$types | Sort-Object | Select-Object -First 8
"... FriendsStrings present: $($types.Contains('osu.Game.Resources.Localisation.Web.FriendsStrings'))"
