// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#if XBOX
namespace osu.Framework.Audio.SDL
{
    /// <summary>
    /// Replacement for ManagedBass' DeviceInfo for the SDL3 audio backend.
    /// </summary>
    public readonly record struct SDLDeviceInfo(string Name, string Driver, bool IsEnabled, bool IsDefault, bool IsInitialized);
}

#endif