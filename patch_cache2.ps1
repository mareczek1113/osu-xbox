$p = 'D:\osu-xbox-port\osu-framework\osu.Framework\Graphics\Shaders\ShaderCompilationStore.cs'
$raw = [IO.File]::ReadAllText($p)
if ($raw.Contains('typeof(ResourceLayoutElementDescription)')) { 'already patched'; exit }
$nl = if ($raw.Contains("`r`n")) { "`r`n" } else { "`n" }
$old = '[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SpirvReflection))]'
if (-not $raw.Contains($old)) { throw 'first patch not found' }
$ind = '                '
$new = $old + $nl +
  $ind + '[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ResourceLayoutDescription))]' + $nl +
  $ind + '[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ResourceLayoutElementDescription))]' + $nl +
  $ind + '[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(VertexElementDescription))]'
$raw = $raw.Replace($old, $new)
[IO.File]::WriteAllText($p, $raw, (New-Object Text.UTF8Encoding($false)))
git -C D:\osu-xbox-port\osu-framework diff -U1 -- osu.Framework/Graphics/Shaders/ShaderCompilationStore.cs
