function Patch([string]$path, [scriptblock]$edit) {
  $raw = [IO.File]::ReadAllText($path)
  $nl = if ($raw.Contains("`r`n")) { "`r`n" } else { "`n" }
  $new = & $edit $raw $nl
  if ($new -eq $raw) { throw "no change made to $path" }
  $bom = ([IO.File]::ReadAllBytes($path)[0] -eq 0xEF)
  [IO.File]::WriteAllText($path, $new, (New-Object Text.UTF8Encoding($bom)))
  "patched $path"
}
function Lines([string]$nl, [string[]]$l) { ($l -join $nl) }

$root = 'D:\osu-xbox-port\osu-framework\osu.Framework\Platform\Xbox'

# ---- UwpHostBridge: text, URI launch ----
Patch "$root\UwpHostBridge.cs" {
  param($raw, $nl)
  if ($raw.Contains('TextReceived')) { return $raw }
  $anchor = '        public void NotifySuspending() => Suspending?.Invoke();'
  if (-not $raw.Contains($anchor)) { throw 'bridge anchor not found' }
  $add = Lines $nl @(
'        /// <summary>',
'        /// Raised when the user has entered text, from a hardware keyboard or the on-screen keyboard. Arguments: the text to insert.',
'        /// </summary>',
'        public event Action<string>? TextReceived;',
'',
'        /// <summary>',
'        /// Called by the framework when it starts (<c>true</c>) or stops (<c>false</c>) wanting text from the user, from a framework thread.',
'        /// The app should show or hide the on-screen keyboard and forward what is typed with <see cref="PostText"/> / <see cref="PostChar"/>.',
'        /// It can be called with <c>true</c> repeatedly while a session is already active, so it must be idempotent.',
'        /// </summary>',
'        public Action<bool>? SetTextInputActive { get; set; }',
'',
'        /// <summary>',
'        /// Asks the system to open a URI (e.g. <c>Launcher.LaunchUriAsync</c>). Returns whether the request was accepted.',
'        /// Called from a framework thread; the app is responsible for marshalling to the UI thread.',
'        /// </summary>',
'        public Func<string, bool>? LaunchUri { get; set; }',
'',
'        private char pendingHighSurrogate;',
'',
'        /// <summary>',
'        /// Call from the CoreWindow <c>CharacterReceived</c> handler with the UTF-16 code unit.',
'        /// Control characters are dropped (backspace, enter and the like arrive as key events), and surrogate pairs are joined.',
'        /// </summary>',
'        public void PostChar(char c)',
'        {',
'            if (char.IsControl(c))',
'                return;',
'',
'            if (char.IsHighSurrogate(c))',
'            {',
'                pendingHighSurrogate = c;',
'                return;',
'            }',
'',
'            if (char.IsLowSurrogate(c))',
'            {',
'                if (pendingHighSurrogate == 0)',
'                    return;',
'',
'                string pair = new string(new[] { pendingHighSurrogate, c });',
'                pendingHighSurrogate = (char)0;',
'                PostText(pair);',
'                return;',
'            }',
'',
'            pendingHighSurrogate = (char)0;',
'            PostText(c.ToString());',
'        }',
'',
'        /// <summary>',
'        /// Call with committed text from the on-screen keyboard or an input method.',
'        /// </summary>',
'        public void PostText(string text)',
'        {',
'            if (!string.IsNullOrEmpty(text))',
'                TextReceived?.Invoke(text);',
'        }',
'',
'')
  $raw.Replace($anchor, $add + $nl + $anchor)
}

# ---- UwpTextInput (new) ----
$ti = Join-Path $root 'UwpTextInput.cs'
if (-not (Test-Path $ti)) {
  $body = @'
// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#if XBOX

using osu.Framework.Input;

namespace osu.Framework.Platform.Xbox
{
    /// <summary>
    /// Text input for the UWP host. The app owns the actual keyboard (hardware characters and the on-screen keyboard) and
    /// forwards text through <see cref="UwpHostBridge"/>; this just starts and stops that session and relays what comes back.
    /// </summary>
    /// <remarks>
    /// The Xbox on-screen keyboard delivers committed text only, so there is no IME composition to forward.
    /// </remarks>
    internal class UwpTextInput : TextInputSource
    {
        private readonly UwpHostBridge bridge;
        private bool listening;

        public UwpTextInput(UwpHostBridge bridge)
        {
            this.bridge = bridge;
        }

        protected override void ActivateTextInput(TextInputProperties properties)
        {
            if (!listening)
            {
                bridge.TextReceived += TriggerTextInput;
                listening = true;
            }

            bridge.SetTextInputActive?.Invoke(true);
        }

        protected override void EnsureTextInputActivated(TextInputProperties properties) => bridge.SetTextInputActive?.Invoke(true);

        protected override void DeactivateTextInput()
        {
            if (listening)
            {
                bridge.TextReceived -= TriggerTextInput;
                listening = false;
            }

            bridge.SetTextInputActive?.Invoke(false);
        }
    }
}

#endif
'@
  [IO.File]::WriteAllText($ti, ($body -replace "`r?`n", "`r`n"), (New-Object Text.UTF8Encoding($false)))
  "created $ti"
}

# ---- UwpGameHost: text input, suspend handling, URL launch ----
Patch "$root\UwpGameHost.cs" {
  param($raw, $nl)
  if ($raw.Contains('onSuspending')) { return $raw }

  $raw = $raw.Replace('using osu.Framework.Extensions.IEnumerableExtensions;', ('using osu.Framework.Extensions.IEnumerableExtensions;' + $nl + 'using osu.Framework.Graphics.Veldrid;'))
  $raw = $raw.Replace('using osu.Framework.Input.Handlers;', ('using osu.Framework.Input;' + $nl + 'using osu.Framework.Input.Handlers;'))

  $ctor = Lines $nl @(
'            this.bridge = bridge;',
'            bridge.Suspending += onSuspending;',
'        }',
'',
'        private void onSuspending()',
'        {',
'            // A suspended app can be terminated without any further notice, so anything still waiting for the delayed background save goes to disk now.',
'            try',
'            {',
'                Config?.Save();',
'            }',
'            catch (Exception e)',
'            {',
'                Logger.Error(e, "Failed to save settings while suspending.");',
'            }',
'',
'            // Direct3D apps on UWP must trim the device when suspending, or the system may terminate them to reclaim memory.',
'            try',
'            {',
'                (Renderer as VeldridRenderer)?.Device?.Trim();',
'            }',
'            catch (Exception e)',
'            {',
'                Logger.Error(e, "Failed to trim the graphics device while suspending.");',
'            }',
'        }',
'',
'        protected override TextInputSource CreateTextInput() => new UwpTextInput(bridge);')
  $old = Lines $nl @('            this.bridge = bridge;', '        }')
  if (-not $raw.Contains($old)) { throw 'ctor anchor not found' }
  $raw = $raw.Replace($old, $ctor)

  $oldUrl = '            // TODO: Windows.System.Launcher.LaunchUriAsync through the bridge.' + $nl + '            Logger.Log("Opening external links is not supported on this platform.", LoggingTarget.Runtime, LogLevel.Important);'
  if (-not $raw.Contains($oldUrl)) { throw 'url anchor not found' }
  $newUrl = Lines $nl @(
'            if (bridge.LaunchUri?.Invoke(url) == true)',
'                return;',
'',
'            Logger.Log($"The system did not open \"{url}\".", LoggingTarget.Runtime, LogLevel.Important);')
  $raw.Replace($oldUrl, $newUrl)
}

# ---- XboxPlatform: say which host uses it ----
Patch "$root\XboxPlatform.cs" {
  param($raw, $nl)
  if ($raw.Contains('UwpGameHost')) { return $raw }
  $old = '    /// </summary>' + $nl + '    internal static class XboxPlatform'
  if (-not $raw.Contains($old)) { throw 'platform anchor not found' }
  $add = '    /// </summary>' + $nl + '    /// <remarks>' + $nl + '    /// Only <see cref="XboxGameHost"/> (the SDL/GDK path) relies on these. <see cref="UwpGameHost"/> takes its storage path and URI launching from <see cref="UwpHostBridge"/>,' + $nl + '    /// so these stay stubs until the GDK path is revived.' + $nl + '    /// </remarks>' + $nl + '    internal static class XboxPlatform'
  $raw.Replace($old, $add)
}
git -C D:\osu-xbox-port diff --stat -- osu-framework
