$p = 'D:\osu-xbox-port\osu-framework\osu.Framework\Graphics\Shaders\ShaderCompilationStore.cs'
$raw = [IO.File]::ReadAllText($p)
if ($raw.Contains('DynamicDependency')) { 'already patched'; exit }
$nl = if ($raw.Contains("`r`n")) { "`r`n" } else { "`n" }
$re = [regex]'(?m)^([ \t]*)private bool tryGetCached<T>\('
if (-not $re.IsMatch($raw)) { throw 'tryGetCached not found' }
$new = $re.Replace($raw, {
  param($m)
  $ind = $m.Groups[1].Value
  $ind + '// Newtonsoft builds SpirvReflection through its parameterized constructor, which Native AOT removes unless it is kept.' + $nl +
  $ind + '[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SpirvReflection))]' + $nl +
  $ind + 'private bool tryGetCached<T>('
}, 1)
[IO.File]::WriteAllText($p, $new, (New-Object Text.UTF8Encoding($false)))
git -C D:\osu-xbox-port\osu-framework diff -U1 -- osu.Framework/Graphics/Shaders/ShaderCompilationStore.cs
