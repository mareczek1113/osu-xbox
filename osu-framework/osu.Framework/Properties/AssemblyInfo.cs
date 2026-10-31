// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Runtime.CompilerServices;
using System.Reflection.Metadata;
#if !XBOX
using osu.Framework.Testing;
#endif

// We publish our internal attributes to other sub-projects of the framework.
// Note, that we omit visual tests as they are meant to test the framework
// behavior "in the wild".

[assembly: InternalsVisibleTo("osu.Framework.Android")]
[assembly: InternalsVisibleTo("osu.Framework.Benchmarks")]
[assembly: InternalsVisibleTo("osu.Framework.iOS")]
[assembly: InternalsVisibleTo("osu.Framework.Tests")]
[assembly: InternalsVisibleTo("osu.Framework.Tests.Dynamic")]
[assembly: InternalsVisibleTo("osu.Framework.Tests.iOS")]
[assembly: InternalsVisibleTo("osu.Framework.Tests.Android")]
#if XBOX
[assembly: InternalsVisibleTo("decoder_test")]
[assembly: InternalsVisibleTo("audio_backend_test")]
#endif
#if !XBOX
[assembly: MetadataUpdateHandler(typeof(HotReloadCallbackReceiver))]
#endif
