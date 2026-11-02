$p = 'D:\osu-xbox-port\osu-framework\osu.Framework\Graphics\Shaders\ShaderCompilationStore.cs'
$raw = [IO.File]::ReadAllText($p)
if ($raw.Contains('aot_keep_instantiations')) { 'already patched'; exit }
$nl = if ($raw.Contains("`r`n")) { "`r`n" } else { "`n" }
$ind = '        '
$anchor = $ind + 'private static readonly Lock save_lock = new Lock();'
if (-not $raw.Contains($anchor)) { throw 'save_lock anchor not found' }
$add = $ind + '// Newtonsoft builds each deserialized array through a temporary List<T> created by reflection. Native AOT only generates List<T> over value types' + $nl +
       $ind + '// when code constructs one, so construct them here to make the instantiations (and their reflection stubs) exist.' + $nl +
       $ind + 'private static readonly object[] aot_keep_instantiations =' + $nl +
       $ind + '{' + $nl +
       $ind + '    new System.Collections.Generic.List<ResourceLayoutDescription>(),' + $nl +
       $ind + '    new System.Collections.Generic.List<ResourceLayoutElementDescription>(),' + $nl +
       $ind + '    new System.Collections.Generic.List<VertexElementDescription>(),' + $nl +
       $ind + '};' + $nl + $nl
$raw = $raw.Replace($anchor, $add + $anchor)
[IO.File]::WriteAllText($p, $raw, (New-Object Text.UTF8Encoding($false)))
'patched'
