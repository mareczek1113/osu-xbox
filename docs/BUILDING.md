# Building and repo layout

## Layout

| Path | What it is |
| --- | --- |
| `uwp-osu/` | The UWP app: entry point, Xbox input handling, settings pages, AOT trimmer root files |
| `osu-aot/` | A patched copy of the osu! source that `uwp-osu` builds against |
| `package-uwp.ps1` | Publishes the app and builds the `.appx` (optionally installs it) |
| `patch_*.ps1` | Scripts that apply the patches to `osu-aot` and the app |
| `gen_*.ps1` | Scripts that generate the trimmer root files and enum dropdown code |
| `run-build22.ps1` | Example of how a build is started |

## Building

Requirements: Windows with the .NET 10 SDK, the Windows 10 SDK (26100) and the Visual Studio C++ build tools (Native AOT needs the linker).

```powershell
.\package-uwp.ps1 -Project uwp-osu -Name UwpOsu -Install -Extra @('-p:XboxPort=true')
```

A full Native AOT build takes roughly 10 to 20 minutes. The package ends up in `uwp-osu\out\UwpOsu.appx`. Install it on the console through Device Portal and set it to "Game".

## Settings added for Xbox

- Settings > Input > Xbox pad: pointer mode, "Menu button pauses"
- Settings > Input > Mouse: separate section and speed for a USB mouse

## Known issues

- USB mouse in pad mode: the console sends no mouse input while its own pointer is off, so hybrid pointer mode is used instead.
- Possible double A press (pad handler plus system pointer press), unconfirmed.

