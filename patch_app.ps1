function Patch([string]$path, [bool]$bom, [scriptblock]$edit) {
  $t = [IO.File]::ReadAllText($path)
  $nl = if ($t.Contains("`r`n")) { "`r`n" } else { "`n" }
  $t = & $edit $t $nl
  [IO.File]::WriteAllText($path, $t, (New-Object Text.UTF8Encoding($bom)))
}
function Rep([string]$t, [string]$old, [string]$new) {
  $i = $t.IndexOf($old)
  if ($i -lt 0) { throw "not found: $old" }
  if ($t.IndexOf($old, $i + 1) -ge 0) { throw "not unique: $old" }
  return $t.Substring(0, $i) + $new + $t.Substring($i + $old.Length)
}

# ---- App.xaml.cs: turn off Xbox system mouse mode ----
Patch 'D:\osu-xbox-scratch\uwp-framework\App.xaml.cs' $true {
  param($t, $nl)
  if ($t.Contains('RequiresPointerMode')) { return $t }
  Rep $t "        InitializeComponent();" ("        InitializeComponent();" + $nl + "        // Xbox otherwise turns the pad into a mouse pointer (A = click) for XAML apps." + $nl + "        RequiresPointerMode = ApplicationRequiresPointerMode.WhenRequested;")
}

# ---- TestGame.cs: test tones ----
Patch 'D:\osu-xbox-scratch\uwp-framework\TestGame.cs' $false {
  param($t, $nl)
  if ($t.Contains('playTone')) { return $t }
  $t = Rep $t 'using osu.Framework.Allocation;' ('using osu.Framework.Allocation;' + $nl + 'using osu.Framework.Audio.Sample;')
  $t = Rep $t '    private double nextLog;' ('    private double nextLog;' + $nl + '    private ISampleStore tones = null!;')
  $t = Rep $t '        report($"pad press {e.Button}");' ('        report($"pad press {e.Button}");' + $nl + '        if (e.Button == JoystickButton.GamePadA) playTone("tone-low");' + $nl + '        if (e.Button == JoystickButton.GamePadB) playTone("tone-high");')
  $t = Rep $t '    [BackgroundDependencyLoader]' ('    private void playTone(string name)' + $nl + '    {' + $nl + '        var sample = tones.Get(name);' + $nl + '        if (sample == null)' + $nl + '        {' + $nl + '            log($"audio: sample {name} not found");' + $nl + '            return;' + $nl + '        }' + $nl + $nl + '        var channel = sample.Play();' + $nl + '        log($"audio: played {name}, channel playing {channel.Playing}, volume {Audio.Volume.Value:0.00}");' + $nl + '    }' + $nl + $nl + '    [BackgroundDependencyLoader]')
  $t = Rep $t '        gameHost = host;' ('        gameHost = host;' + $nl + '        tones = Audio.GetSampleStore(new ToneStore());')
  $t = Rep $t '            cursor = new Box { Size = new Vector2(16), Origin = Anchor.Centre, Colour = Color4.Cyan },' ('            cursor = new Box { Size = new Vector2(16), Origin = Anchor.Centre, Colour = Color4.Cyan },')
  $t = Rep $t '    protected override void Update()' ('    protected override void LoadComplete()' + $nl + '    {' + $nl + '        base.LoadComplete();' + $nl + '        // Audible check without any input: a mid tone 1.5 s after start.' + $nl + '        Scheduler.AddDelayed(() => playTone("tone-mid"), 1500);' + $nl + '    }' + $nl + $nl + '    protected override void Update()')
  $t
}
'ok'
