// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Input;
using osu.Framework.Input.Handlers;
using osu.Framework.Input.Handlers.Joystick;
using osu.Framework.Input.Handlers.Keyboard;
#if !XBOX
using osu.Framework.Input.Handlers.Midi;
#endif
using osu.Framework.Input.Handlers.Mouse;
using osu.Framework.Input.Handlers.Pen;
#if !XBOX
using osu.Framework.Input.Handlers.Tablet;
#endif
using osu.Framework.Input.Handlers.Touch;
using osu.Framework.Platform.SDL2;
using osu.Framework.Platform.SDL3;
using SixLabors.ImageSharp.Formats.Png;

namespace osu.Framework.Platform
{
    public abstract class SDLGameHost : GameHost
    {
        public override bool CapsLockEnabled => (Window as ISDLWindow)?.CapsLockPressed == true;

        public override bool OnScreenKeyboardOverlapsGameWindow => (Window as ISDLWindow)?.KeyboardAttached == false;

        protected SDLGameHost(string gameName, HostOptions? options = null)
            : base(gameName, options)
        {
        }

        protected override TextInputSource CreateTextInput()
        {
            if (Window is ISDLWindow window)
                return new SDLWindowTextInput(window);

            return base.CreateTextInput();
        }

        protected override Clipboard CreateClipboard()
            => FrameworkEnvironment.UseSDL3
                ? new SDL3Clipboard(PngFormat.Instance) // PNG works well on linux
                : new SDL2Clipboard();

#if !XBOX
        protected override IEnumerable<InputHandler> CreateAvailableInputHandlers()
        {
            yield return new KeyboardHandler();
            // tablet should get priority over mouse to correctly handle cases where tablet drivers report as mice as well.
            yield return new OpenTabletDriverHandler();

            if (FrameworkEnvironment.UseSDL3)
                yield return new PenHandler();

            yield return new MouseHandler();
            yield return new TouchHandler();
            yield return new JoystickHandler();
            yield return new MidiHandler();
        }
#endif
    }
}
