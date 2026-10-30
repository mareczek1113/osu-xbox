// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#if XBOX

using System;
using System.Collections.Generic;
using osu.Framework.Extensions;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Input.Handlers;
using osu.Framework.Input.Handlers.Joystick;
using osu.Framework.Input.Handlers.Keyboard;
using osu.Framework.Input.Handlers.Mouse;
using osu.Framework.Logging;

namespace osu.Framework.Platform.Xbox
{
    /// <summary>
    /// Game host for Xbox consoles (GDK). Built only with <c>-p:XboxPort=true</c>.
    /// Derives from <see cref="SDLGameHost"/> (windowing, input and audio-device plumbing via SDL3) rather than
    /// <see cref="DesktopGameHost"/>, which depends on desktop-only facilities (named-pipe IPC, shell execute, etc.).
    /// </summary>
    /// <remarks>
    /// Anything that needs the GDK itself lives behind <see cref="XboxPlatform"/>, so that the rest of the host stays plain C#.
    /// </remarks>
    public class XboxGameHost : SDLGameHost
    {
        internal XboxGameHost(string gameName, HostOptions? options)
            : base(gameName, options)
        {
        }

        protected override IWindow CreateWindow(GraphicsSurfaceType preferredSurface)
        {
            if (!FrameworkEnvironment.UseSDL3)
                throw new NotSupportedException("The Xbox host requires SDL3.");

            return new SDL3XboxWindow(preferredSurface, Options.FriendlyGameName);
        }

        // There is no system clipboard to speak of on console.
        protected override Clipboard CreateClipboard() => new HeadlessClipboard();

        protected override IEnumerable<InputHandler> CreateAvailableInputHandlers()
        {
            // Controller first; keyboard and mouse are supported on console but optional.
            // Tablet (OpenTabletDriver), MIDI and pen handlers are intentionally absent: they rely on desktop native USB/driver stacks.
            yield return new JoystickHandler();
            yield return new KeyboardHandler();
            yield return new MouseHandler();
        }

        public override IEnumerable<string> UserStoragePaths => XboxPlatform.GetPersistentStoragePath().Yield();

        public override Storage GetStorage(string path) => new NativeStorage(path, this);

        // There is no shell to hand files or URLs to.
        public override bool OpenFileExternally(string filename) => false;

        public override bool PresentFileExternally(string filename) => false;

        public override void OpenUrlExternally(string url)
        {
            if (!url.CheckIsValidUrl())
                throw new ArgumentException("The provided URL must be one of either http://, https:// or mailto: protocols.", nameof(url));

            if (!XboxPlatform.TryLaunchUrl(url))
                Logger.Log("Opening external links is not supported on this platform.", LoggingTarget.Runtime, LogLevel.Important);
        }
    }
}

#endif