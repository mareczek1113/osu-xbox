// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#if XBOX

using System;

namespace osu.Framework.Platform.Xbox
{
    /// <summary>
    /// The single seam between the framework and the Xbox GDK. Each member is a stub to be filled in against the GDK documentation.
    /// Keeping the surface this small means the rest of the port stays free of platform-specific code.
    /// </summary>
    internal static class XboxPlatform
    {
        /// <summary>
        /// A writable directory that persists between launches (settings, beatmaps, scores).
        /// </summary>
        /// <remarks>
        /// TODO(GDK): replace with the title's persistent local storage location.
        /// The environment variable and the local-app-data fallback below are only suitable for running the host on a desktop machine while developing.
        /// </remarks>
        public static string GetPersistentStoragePath()
            => Environment.GetEnvironmentVariable("OSU_XBOX_STORAGE")
               ?? Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create);

        /// <summary>
        /// Asks the system to open <paramref name="url"/>.
        /// </summary>
        /// <returns>Whether the system accepted the request.</returns>
        /// <remarks>TODO(GDK): hook up the platform URI launch API, if available to this title.</remarks>
        public static bool TryLaunchUrl(string url) => false;
    }
}

#endif