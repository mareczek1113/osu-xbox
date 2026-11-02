 <div align="center">

<img src="https://github.com/ppy/osu/blob/master/assets/lazer.png" width="100" alt="osu! logo">

# osu! Xbox

### Bringing osu!lazer to Xbox Series X|S.

An unofficial port bringing osu!lazer to Xbox Series X|S using UWP/GDK technologies.

<br>

![Status](https://img.shields.io/badge/STATUS-WORKING_PORT-ff66aa?style=for-the-badge)
![Platform](https://img.shields.io/badge/PLATFORM-XBOX_SERIES_X%7CS-ff66aa?style=for-the-badge)
![Language](https://img.shields.io/badge/LANGUAGE-C%23-ff66aa?style=for-the-badge)
![Project](https://img.shields.io/badge/PROJECT-OSU!LAZER-ff66aa?style=for-the-badge)

<br>

**The full game is running on Xbox Series S, with graphics and audio working. Menus, the pointer, controller input and USB mouse support are in place and being polished.**

</div>

---

## 💗 About

osu! Xbox is an independent project bringing [osu!lazer](https://github.com/ppy/osu) to Xbox Series X|S.

The project adapts osu!'s existing codebase and framework to work within an Xbox environment.

The full game is running on Xbox Series S, with graphics rendering and audio playback working.

Xbox Series X is also a target platform, with compatibility testing still to be completed.

The current focus is finishing input support and working towards beatmap and skin support.

> **Current milestone:** osu!lazer runs on Xbox Series S. Graphics and audio are working, and mouse, pointer and controller input are almost complete.

## ✨ Goals

- 🎮 Complete Xbox controller support.
- 🖱️ Finish mouse and pointer input.
- 🎵 Maintain reliable audio playback and timing.
- 🎨 Keep graphics rendering working across Xbox hardware.
- 🗺️ Add beatmap support.
- 🎨 Add skin support.
- ⚡ Improve performance and stability.
- 🖥️ Make the interface comfortable to use on a television.
- 🔧 Maintain compatibility with the adapted osu!framework.
- 💗 Preserve the core osu!lazer gameplay experience.

## 🛠️ Technology

| Component | Technology |
|---|---|
| Original game | osu!lazer |
| Main language | C# |
| Framework | Modified osu!framework |
| Target platform | Xbox Series X\|S |
| Platform technology | UWP/GDK |
| Graphics | Working Xbox graphics implementation |
| Audio | Working Xbox audio implementation |
| Compilation | .NET / NativeAOT |

## 📊 Development Status

| Component | Status |
|---|---|
| Xbox application startup | 🟢 Working |
| Full osu!lazer integration | 🟢 Working |
| Graphics rendering | 🟢 Working |
| Audio playback | 🟢 Working |
| Xbox platform integration | 🟢 Working |
| Framework adaptation | 🟢 Working |
| Controller pointer and menus | 🟢 Working |
| USB mouse | 🟡 Being verified on hardware |
| Controller-based gameplay | 🟡 Playable, being polished |
| Beatmap support | 🔴 Not started |
| Skin support | 🔴 Not started |
| Xbox Series X testing | 🟡 Not yet verified |

**Status key**

- 🟢 Working
- 🟡 Almost complete / in progress
- 🔴 Not started / not completed

## 🎮 Current Focus: Input Support

With the full game running on Xbox and graphics and audio operational, development is focused on input support.

### Controller pointer and menus

With a controller, the Xbox acts as a mouse-driven osu! machine:

| Input | Action |
|---|---|
| Left stick (default) | Move the cursor |
| Right stick (default) | Scroll menus |
| A | Left click |
| Left trigger (held) | Precision mode, slows the cursor down |
| Menu button | Pause while playing, back in menus (can be switched off) |
| View button | Open the on-screen keyboard when a text box is active |

Open **Settings → Input → Gamepad (UWP)** to tune it:

- **Move pointer with right stick**: swaps the sticks, so the right stick moves the cursor and the left stick scrolls.
- **Pointer speed**, **response curve** and **precision speed**.
- **Menu button pauses**: also sends Escape, which pauses a beatmap and goes back in menus.
- **Menu scroll speed** and **stick deadzone**.

The settings are saved automatically.

### USB mouse

A USB mouse moves the cursor from its raw movement, and its buttons and wheel work as normal. Sensitivity is in **Settings → Input → Mouse (UWP)**. The pad and the mouse share one cursor, so switching between them continues from where the cursor is.

Pointer mode is chosen with `pointer.txt` in the app's local folder (`hybrid` by default, also `pad` and `system`). Use `hybrid` for a USB mouse.

### Still in progress

- Polishing controller gameplay input and verifying USB mouse behaviour on hardware.
- Beatmap and skin support.
## 🧪 Technical Development

### Graphics

The Xbox graphics implementation is working, allowing osu!lazer to render on Xbox Series S.

### Audio

Audio playback is working on Xbox, allowing the game to produce sound through its adapted audio implementation.

### Platform compatibility

The application and adapted framework are running in the Xbox environment.

The project targets Xbox Series X|S, with Series X testing still to be completed.

### Input

The controller pointer, menus and Menu-button pause work, and USB mouse support is being verified. Controller gameplay is playable and being polished.

### Beatmaps and skins

Beatmap and skin support have not started yet. These are planned features for future development.

## 📦 Installation

<badge color="warning">Coming soon</badge>

There is currently no public installation package. The download link below is a placeholder until the first release is available.

**[Download osu! Xbox — Coming Soon](https://github.com/mareczek1113/osu-xbox/releases)**

### Installation guide

Once a release is available, follow these steps:

**1. Set up Xbox Developer Mode**

Enable Developer Mode on your Xbox Series X|S.

Follow this tutorial for help:

[Watch the Xbox Developer Mode setup tutorial](https://www.youtube.com/watch?v=ABdUwiBK1s0)

**2. Download and install osu! Xbox**

- Download the installation package from the releases page.
- Upload the package through Xbox Device Portal.
- Install the package on your Xbox.

<text color="secondary" size="xs">The installation package and download link will be added when the first public release is ready.</text>

**3. Change the UWP application type**

Change the UWP application type from **App** to **Game** as required for the Xbox setup.

**4. Launch osu! Xbox**

Once installed and configured, launch osu! Xbox from your Xbox Dev Mode environment.

## 🗺️ Roadmap

- [x] Create the project repository.
- [x] Adapt osu!framework for Xbox.
- [x] Get the Xbox application running.
- [x] Integrate osu!lazer.
- [x] Get graphics rendering working.
- [x] Get audio playback working.
- [x] Run the full game on Xbox Series S.
- [x] Controller pointer, menu navigation and pause.
- [x] Choice of left or right stick for the pointer.
- [ ] Finish USB mouse support.
- [ ] Complete Xbox controller support.
- [ ] Complete controller-based gameplay.
- [ ] Add beatmap support.
- [ ] Add skin support.
- [ ] Verify the build on Xbox Series X.
- [ ] Polish input behaviour and user experience.
- [ ] Test stability and performance.
- [ ] Prepare a public release.

## 🤝 Contributing

Suggestions, technical discussion and contributions are welcome.

Please open an issue to discuss significant changes before submitting a pull request.

All contributions must respect the applicable licences of the original project and third-party dependencies.

## 💗 Credits

- [osu!](https://osu.ppy.sh/) — Original rhythm game.
- [ppy/osu](https://github.com/ppy/osu) — Open-source osu!lazer project.
- [ppy/osu-framework](https://github.com/ppy/osu-framework) — Game framework.

This project is independently developed for the osu! community and is not affiliated with, endorsed by, or officially supported by ppy or the osu! development team.

## 📄 Licence

The licensing terms for this port and its original components must be respected.

See [LICENSE](LICENSE) for the licence applicable to this repository.

---

<div align="center">

**Made with 💗 for the osu! community.**

*osu! Xbox — an independent project by fans of osu!*

</div>
