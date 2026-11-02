# Generates uwp-osu\RealmModels.xml: an ILLink/ILC root descriptor that keeps only osu.Game's Realm model types
# (instead of rooting all of osu.Game, which made ILC run out of memory).
$root = 'D:\osu-xbox-scratch\osu-aot\osu.Game'
$files = 'Beatmaps\BeatmapDifficulty.cs','Beatmaps\BeatmapInfo.cs','Beatmaps\BeatmapMetadata.cs','Beatmaps\BeatmapSetInfo.cs',
  'Beatmaps\BeatmapUserSettings.cs','Collections\BeatmapCollection.cs','Configuration\RealmRulesetSetting.cs',
  'Input\Bindings\RealmKeyBinding.cs','Models\RealmFile.cs','Models\RealmNamedFileUsage.cs','Models\RealmOnlineAsset.cs',
  'Models\RealmUser.cs','Rulesets\Mods\ModPreset.cs','Rulesets\RulesetInfo.cs','Scoring\ScoreInfo.cs','Skinning\SkinInfo.cs'
$types = New-Object System.Collections.Generic.List[string]
foreach ($f in $files) {
  $t = Get-Content "$root\$f" -Raw
  $ns = [regex]::Match($t, '(?m)^namespace\s+([\w\.]+)').Groups[1].Value
  foreach ($m in [regex]::Matches($t, '(?m)^\s*public\s+(?:partial\s+|sealed\s+)*class\s+(\w+)[^{;]*:\s*[^{;]*\b(RealmObject|EmbeddedObject|IRealmObject|IEmbeddedObject)\b')) {
    $types.Add("$ns.$($m.Groups[1].Value)")
  }
}
$xml = "<linker>`n  <assembly fullname=`"osu.Game`">`n" + (($types | Sort-Object -Unique | ForEach-Object { "    <type fullname=`"$_`" preserve=`"all`" />" }) -join "`n") + "`n  </assembly>`n</linker>`n"
Set-Content -Path D:\osu-xbox-scratch\uwp-osu\RealmModels.xml -Value $xml -Encoding UTF8
$types | Sort-Object -Unique
