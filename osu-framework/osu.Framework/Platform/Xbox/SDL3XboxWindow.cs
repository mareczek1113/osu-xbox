// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#if XBOX

using osu.Framework.Platform.SDL3;

namespace osu.Framework.Platform.Xbox
{
    /// <summary>
    /// SDL3 window for Xbox. The console presents a single fullscreen display, so none of the desktop window
    /// chrome / IME / icon handling from <see cref="Windows.SDL3WindowsWindow"/> applies here.
    /// </summary>
    internal class SDL3XboxWindow : SDL3DesktopWindow
    {
        public SDL3XboxWindow(GraphicsSurfaceType surfaceType, string appName)
            : base(surfaceType, appName)
        {
        }
    }
}

#endif