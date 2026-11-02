function Patch([string]$path, [scriptblock]$edit) {
  $raw = [IO.File]::ReadAllText($path)
  $nl = if ($raw.Contains("`r`n")) { "`r`n" } else { "`n" }
  $new = & $edit $raw $nl
  if ($new -eq $raw) { throw "no change made to $path" }
  $bom = ([IO.File]::ReadAllBytes($path)[0] -eq 0xEF); [IO.File]::WriteAllText($path, $new, (New-Object Text.UTF8Encoding($bom)))
  "patched $path"
}

$root = 'D:\osu-xbox-port\veldrid\src\Veldrid'

Patch "$root\GraphicsDevice.cs" {
  param($raw, $nl)
  if ($raw.Contains('public virtual void Trim()')) { return $raw }
  $re = [regex]'(?m)^([ \t]*)public abstract void ResetFence\(Fence fence\);'
  if (-not $re.IsMatch($raw)) { throw 'ResetFence anchor not found' }
  $re.Replace($raw, {
    param($m)
    $i = $m.Groups[1].Value
    $m.Value + $nl + $nl +
    $i + '/// <summary>' + $nl +
    $i + '///     Releases memory the graphics driver holds for temporary use. Backends without such a concept ignore this.' + $nl +
    $i + '///     Direct3D 11 on UWP requires it to be called when the app is suspended.' + $nl +
    $i + '/// </summary>' + $nl +
    $i + 'public virtual void Trim()' + $nl +
    $i + '{' + $nl +
    $i + '}'
  }, 1)
}

Patch "$root\D3D11\D3D11GraphicsDevice.cs" {
  param($raw, $nl)
  if ($raw.Contains('public override void Trim()')) { return $raw }
  $re = [regex]'(?m)^([ \t]*)private protected override void WaitForIdleCore\(\)'
  if (-not $re.IsMatch($raw)) { throw 'WaitForIdleCore anchor not found' }
  $re.Replace($raw, {
    param($m)
    $i = $m.Groups[1].Value
    $i + 'public override void Trim()' + $nl +
    $i + '{' + $nl +
    $i + '    // IDXGIDevice3 is not available on very old runtimes; there is nothing to trim there.' + $nl +
    $i + '    using var dxgiDevice = device.QueryInterfaceOrNull<IDXGIDevice3>();' + $nl +
    $i + '    dxgiDevice?.Trim();' + $nl +
    $i + '}' + $nl + $nl +
    $m.Value
  }, 1)
}

git -C D:\osu-xbox-port\veldrid diff --stat
