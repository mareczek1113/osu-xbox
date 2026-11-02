$p = 'D:\osu-xbox-port\osu-framework\osu.Framework\Graphics\Shaders\ShaderCompilationStore.cs'
$raw = [IO.File]::ReadAllText($p)
if ($raw.Contains('private void saveToCache') -and $raw -match '(?s)DynamicDependency[^\r\n]*\r?\n\s*private void saveToCache') { 'already patched'; exit }
$nl = if ($raw.Contains("`r`n")) { "`r`n" } else { "`n" }
$ind = '        '
# 1. save path needs the same keep attributes, otherwise Newtonsoft finds no members and writes "Reflection":{}
$attrs = @(
  '[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SpirvReflection))]',
  '[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ResourceLayoutDescription))]',
  '[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ResourceLayoutElementDescription))]',
  '[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(VertexElementDescription))]'
)
$block = ($attrs | ForEach-Object { $ind + $_ }) -join $nl
$comment = $ind + '// Native AOT strips the reflection metadata Newtonsoft needs to write SpirvReflection, which produced empty "Reflection":{} cache entries.' + $nl
$re = [regex]'(?m)^[ \t]*private void saveToCache\('
if (-not $re.IsMatch($raw)) { throw 'saveToCache not found' }
$raw = $re.Replace($raw, ($comment + $block + $nl + $ind + 'private void saveToCache('), 1)
# 2. invalidate entries written by earlier AOT builds
$old = 'private const int cache_version = 1;'
if (-not $raw.Contains($old)) { throw 'cache_version not found' }
$raw = $raw.Replace($old, 'private const int cache_version = 2;')
[IO.File]::WriteAllText($p, $raw, (New-Object Text.UTF8Encoding($false)))
git -C D:\osu-xbox-port\osu-framework diff -U1 -- osu.Framework/Graphics/Shaders/ShaderCompilationStore.cs
