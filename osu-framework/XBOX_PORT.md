# Xbox (GDK) port notes

Build the framework for the port with `-p:XboxPort=true` (defines `XBOX`). The normal desktop build is unchanged.

## What the Xbox build strips
- NUnit / `Testing` (visual-test harness, hot reload)
- Desktop hosts and platform glue: `DesktopGameHost`, Windows/Linux/macOS/Apple hosts and windows, named-pipe/TCP IPC, headless host
- Tablet (OpenTabletDriver) and MIDI input handlers and their packages
- `Reflection.Emit` in `TransformCustom` (reflection path is always used)

## What was added
- `Platform/Xbox/XboxGameHost.cs` - SDL3-based host, controller-first input
- `Platform/Xbox/SDL3XboxWindow.cs` - minimal window
- `Platform/Xbox/XboxPlatform.cs` - the only place that should touch the GDK (storage path, URL launch). Stubs to fill in.
- `Host.GetSuitableXboxHost(gameName, options)` - entry point

## Dev-loop tip
`XboxPlatform.GetPersistentStoragePath()` honours the `OSU_XBOX_STORAGE` environment variable so the host can be run on a desktop PC while developing.

## Not done yet
- Renderer: only a Veldrid Direct3D11 backend exists (no D3D12). Check whether the GDK allows D3D11 on the target console before writing a D3D12 backend.
- Shaders: compiled at runtime today; consoles likely need offline-compiled shaders.
- Audio (BASS) and video (FFmpeg) native libraries need GDK builds or replacements.
- Remaining NativeAOT analyzer warnings: Newtonsoft.Json, vertex-layout reflection, a few reflection-based DI fallbacks.

## Audio (SDL3 backend, replaces BASS at runtime in the Xbox build)
- Audio/SDL/AudioDecoder.cs: managed WAV / Ogg Vorbis / MP3 decoding (NVorbis, NLayer 3.0.0), checked against BASS: identical length and zero offset on the test files.
- SDLAudioOutput (one SDL3 output stream, 48 kHz stereo float), SDLAudioMixer, SampleSDL* (volume, balance, frequency, looping, playback concurrency).
- Measured on a PC (WASAPI shared): Play() to first audio rendered ~9 ms median, plus the 10 ms device buffer.
- Audio/SDL/TrackSDL.cs: tracks, fully decoded in memory. Start/stop/seek (applied lock-free on the device thread, CurrentTime reflects a seek immediately), Completed fires once, looping with RestartPoint, volume, balance, frequency (including negative = reverse). Checked end to end through AudioManager on a PC.
- Not done yet: **tempo is plain resampling, so pitch follows speed** (DT/HT will sound pitched; needs a time-stretcher such as WSOLA), audio effects (low-pass etc.), amplitude data (`CurrentAmplitudes`, used by the logo visualiser), waveform decoding, named device selection.
- Memory: a decoded 4-minute track is roughly 85-90 MB as 32-bit float. Consider keeping tracks as 16-bit PCM if the console's memory budget gets tight.
