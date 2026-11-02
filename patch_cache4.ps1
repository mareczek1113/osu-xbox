$p = 'D:\osu-xbox-port\osu-framework\osu.Framework\Graphics\Shaders\ShaderCompilationStore.cs'
$raw = [IO.File]::ReadAllText($p)
if ($raw.Contains('List<ResourceLayoutDescription>')) { 'already patched'; exit }
$nl = if ($raw.Contains("`r`n")) { "`r`n" } else { "`n" }
$ind = '        '
$anchor = $ind + '[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(VertexElementDescription))]' + $nl + $ind + 'private bool tryGetCached<T>('
if (-not $raw.Contains($anchor)) { throw 'tryGetCached anchor not found' }
$add = $ind + '[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(System.Collections.Generic.List<ResourceLayoutDescription>))]' + $nl +
       $ind + '[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(System.Collections.Generic.List<ResourceLayoutElementDescription>))]' + $nl +
       $ind + '[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(System.Collections.Generic.List<VertexElementDescription>))]' + $nl
$raw = $raw.Replace($anchor, $ind + '[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(VertexElementDescription))]' + $nl + $add + $ind + 'private bool tryGetCached<T>(')
[IO.File]::WriteAllText($p, $raw, (New-Object Text.UTF8Encoding($false)))
git -C D:\osu-xbox-port\osu-framework diff -U0 -- osu.Framework/Graphics/Shaders/ShaderCompilationStore.cs | Select-String 'List<'
