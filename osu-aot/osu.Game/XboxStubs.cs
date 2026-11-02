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