# Applies the Xbox-port build patches to the scratch copy of osu-source (D:\osu-xbox-scratch\osu-aot).
# Idempotent. Does NOT touch D:\osu-xbox-port\osu-source.
$ErrorActionPreference = 'Stop'
$root  = 'D:\osu-xbox-scratch\osu-aot'
$game  = Join-Path $root 'osu.Game'
$fwCsproj = 'D:\osu-xbox-port\osu-framework\osu.Framework\osu.Framework.csproj'

function Edit-File($path, $old, $new) {
    $t = [IO.File]::ReadAllText($path)
    if ($new -ne '' -and $t.Contains($new)) { Write-Output "already patched: $path"; return }
    if (-not $t.Contains($old)) {
        if ($new -eq '') { Write-Output "already patched: $path"; return }
        throw "pattern not found in $path : $old"
    }
    [IO.File]::WriteAllText($path, $t.Replace($old, $new), (New-Object Text.UTF8Encoding($true)))
    Write-Output "patched: $path"
}

# 1. Framework fork: keep the tiny, self-contained TestingExtensions (ChildrenOfType) that game code uses.
Edit-File $fwCsproj '<Compile Remove="Testing\**" />' ('<Compile Remove="Testing\**" />' + "`r`n" + '    <Compile Include="Testing\TestingExtensions.cs" />')

# 2. osu.Game csproj: XBOX define, drop test scenes and the tablet settings UI.
$props = @'
  <!-- Xbox port: build with -p:XboxPort=true -->
  <PropertyGroup Condition="'$(XboxPort)' == 'true'">
    <DefineConstants>$(DefineConstants);XBOX</DefineConstants>
  </PropertyGroup>
  <ItemGroup Condition="'$(XboxPort)' == 'true'">
    <Compile Remove="Tests\**" />
    <Compile Remove="Overlays\Settings\Sections\Input\TabletSettings.cs" />
    <Compile Remove="Overlays\Settings\Sections\Input\TabletAreaSelection.cs" />
    <Compile Remove="Overlays\Settings\Sections\Input\RotationPresetButtons.cs" />
  </ItemGroup>
</Project>
'@
Edit-File (Join-Path $game 'osu.Game.csproj') '</Project>' $props

# 3. Tablet settings are not available: no settings subsection for tablet handlers.
Edit-File (Join-Path $game 'OsuGameBase.cs') "case ITabletHandler th:`r`n                        return new TabletSettings(th);" "case ITabletHandler:`r`n                        return null;"

# 4. Stubs for framework types the fork strips (desktop-only). Never instantiated on the console.
$stubs = @'
// Xbox port: stand-ins for framework types removed from the fork when XboxPort=true.
// They only exist so desktop-only code in osu.Game still compiles; none of it is active on the console.
#nullable enable

using osu.Framework.Bindables;
using osu.Framework.Input.Handlers;
using osuTK;

namespace osu.Framework.Platform
{
    public abstract class DesktopGameHost : GameHost
    {
        protected DesktopGameHost(string gameName, HostOptions? options = null)
            : base(gameName, options)
        {
        }
    }

    public class DesktopStorage : NativeStorage
    {
        public DesktopStorage(string path, DesktopGameHost? host)
            : base(path, host)
        {
        }
    }
}

namespace osu.Framework.Platform.Windows
{
    public enum FullscreenCapability
    {
        Unknown,
        Incapable,
        Capable
    }

    public interface IWindowsRenderer
    {
        IBindable<FullscreenCapability> FullscreenCapability { get; }
    }
}

namespace osu.Framework.Input.Handlers.Tablet
{
    public class TabletInfo
    {
        public string Name { get; } = string.Empty;
        public Vector2 Size { get; }
    }

    public interface ITabletHandler
    {
        BindableBool Enabled { get; }
        IBindable<TabletInfo?> Tablet { get; }
        Bindable<Vector2> OutputAreaSize { get; }
        Bindable<Vector2> OutputAreaOffset { get; }
    }
}

namespace osu.Framework.Input.Handlers.Midi
{
    public abstract class MidiHandler : InputHandler
    {
    }
}
'@
$stubPath = Join-Path $game 'XboxStubs.cs'
[IO.File]::WriteAllText($stubPath, $stubs, (New-Object Text.UTF8Encoding($true)))
Write-Output "wrote: $stubPath"

# 2b. Two helpers under Tests/ are used by shipping code (offline API chat connector, first-run UI scale screen).
Edit-File (Join-Path $game 'osu.Game.csproj') '<Compile Remove="Tests\**" />' ('<Compile Remove="Tests\**" />' + "`r`n" + '    <Compile Include="Tests\TestChatClientConnector.cs" />' + "`r`n" + '    <Compile Include="Tests\Visual\DependencyProvidingContainer.cs" />' + "`r`n" + '    <Compile Include="Tests\PollingChatClient.cs" />')


# 6. Sentry: no error reporting on the console (item 10f). Its native lib needs version.dll, which UWP does not have.
#    Also drop NUnit (only used by the excluded Tests folder).
$csp = Join-Path $game 'osu.Game.csproj'
$t = [IO.File]::ReadAllText($csp)
$t = $t.Replace('<PackageReference Include="Sentry" Version="6.11.0"/>', '<PackageReference Include="Sentry" Version="6.11.0" Condition="''$(XboxPort)'' != ''true''" />')
$t = $t.Replace('<PackageReference Include="NUnit" Version="4.6.1"/>', '<PackageReference Include="NUnit" Version="4.6.1" Condition="''$(XboxPort)'' != ''true''" />')
if (-not $t.Contains('Utils\SentryLogger.cs')) { $t = $t.Replace('<Compile Remove="Tests\**" />', '<Compile Remove="Tests\**" />' + "`r`n" + '    <Compile Remove="Utils\SentryLogger.cs" />') }
[IO.File]::WriteAllText($csp, $t, (New-Object Text.UTF8Encoding($true)))

$stub = @"
// Xbox port: no Sentry on the console. Same public surface as Utils/SentryLogger.cs, minus the SDK.
#nullable enable

using System;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using osu.Framework.Bindables;
using osu.Framework.Platform;
using osu.Game.Online.API.Requests.Responses;

namespace osu.Game.Utils
{
    public class SentryLogger : IDisposable
    {
        public SentryLogger(OsuGame game, Storage? storage = null)
        {
        }

        public void AttachUser(IBindable<APIUser> user)
        {
        }

        public static bool IsLocalUserConnectivityException(Exception exception)
        {
            switch (exception)
            {
                case TimeoutException te:
                    return te.Message.Contains(@"elapsed without receiving a message from the server");

                case WebException we:
                    return we.Status == WebExceptionStatus.Timeout;

                case WebSocketException:
                case SocketException:
                    return true;
            }

            return false;
        }

        public void Dispose()
        {
        }
    }
}
"@
[IO.File]::WriteAllText((Join-Path $game 'Utils\XboxSentryLogger.cs'), $stub, (New-Object Text.UTF8Encoding($true)))

$og = Join-Path $game 'OsuGame.cs'
$t = [IO.File]::ReadAllText($og)
$t = [regex]::Replace($t, '(?m)^using Sentry;\r?\n', '')
$t = [regex]::Replace($t, '(?s)\s*SentrySdk\.ConfigureScope\(scope =>\s*\{.*?\}\);\r?\n', "`r`n")
[IO.File]::WriteAllText($og, $t, (New-Object Text.UTF8Encoding($true)))
Write-Output 'sentry removed'
