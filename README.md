 <div align="center">

<img src="https://github.com/ppy/osu/blob/master/assets/lazer.png" width="100" alt="osu! logo">

# osu! Xbox

### Bringing osu!lazer to Xbox Series X|S.

An unofficial port bringing osu!lazer to Xbox Series X|S using GDK technologies.

<br>

![Status](https://img.shields.io/badge/STATUS-WORKING_PORT-ff66aa?style=for-the-badge)
![Platform](https://img.shields.io/badge/PLATFORM-XBOX_SERIES_X%7CS-ff66aa?style=for-the-badge)
![Language](https://img.shields.io/badge/LANGUAGE-C%23-ff66aa?style=for-the-badge)
![Project](https://img.shields.io/badge/PROJECT-OSU!LAZER-ff66aa?style=for-the-badge)

<br>

**The full game is running on Xbox Series S, with graphics and audio working. Controller pointer support, menu navigation and controller-based gameplay are working and being polished.**

</div>

---

## 💗 About

osu! Xbox is an independent project bringing [osu!lazer](https://github.com/ppy/osu) to Xbox Series X|S.

The project adapts osu!'s existing codebase and framework to work within an Xbox environment using GDK.

The full game is running on Xbox Series S, with graphics rendering and audio playback working.

Xbox Series X is also a target platform, with compatibility testing still to be completed.

The current focus is polishing controller input and working towards beatmap and skin support.

> **Current milestone:** osu!lazer runs on Xbox Series S. Graphics, audio, controller pointer navigation, menus and controller-based gameplay are working.

## ✨ Goals

- 🎮 Polish Xbox controller support.
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
| Platform technology | GDK |
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
| Controller-based gameplay | 🟡 Almost complete |
| Beatmap support | 🔴 Not started |
| Skin support | 🔴 Not started |
| Xbox Series X testing | 🟡 Not yet verified |

**Status key**

- 🟢 Working
- 🟡 Almost complete / in progress
- 🔴 Not started / not completed

## 🎮 Current Focus: Controller Support

With the full game running on Xbox and graphics and audio operational, development is focused on polishing controller input.

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

Open **Settings → Input → Gamepad (GDK)** to tune it:

- **Move pointer with right stick**: swaps the sticks, so the right stick moves the cursor and the left stick scrolls.
- **Pointer speed**, **response curve** and **precision speed**.
- **Menu button pauses**: also sends Escape, which pauses a beatmap and goes back in menus.
- **Menu scroll speed** and **stick deadzone**.

### Mouse support limitation

Native mouse support is not currently available in this port.

The existing input implementation relies on UWP-specific APIs, which are not available in the same way under the current GDK application setup. As a result, adding mouse support requires a different input implementation rather than simply enabling USB mouse support.

The current focus is therefore on controller-based navigation and gameplay.

### Controller-based gameplay

Controller-based gameplay is almost complete. The remaining work focuses on polishing input behaviour and ensuring a consistent gameplay experience.

## 🧪 Technical Development

### Graphics

The Xbox graphics implementation is working, allowing osu!lazer to render on Xbox Series S.

### Audio

Audio playback is working on Xbox, allowing the game to produce sound through its adapted audio implementation.

### Platform compatibility

The application and adapted framework are running in the Xbox environment using GDK.

The project targets Xbox Series X|S, with Series X testing still to be completed.

### Input

Controller pointer movement, menu navigation and controller-based gameplay are working. Development is focused on polishing the remaining input behaviour.

Native mouse support is not currently implemented because of the limitations of the existing UWP-specific input implementation.

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

**3. Configure the application**

Configure the GDK application for the appropriate Xbox game deployment and launch settings.

**4. Launch osu! Xbox**

Once installed and configured, launch osu! Xbox from your Xbox development environment.

## 🗺️ Roadmap

- [x] Create the project repository.
- [x] Adapt osu!framework for Xbox.
- [x] Get the Xbox application running.
- [x] Integrate osu!lazer.
- [x] Get graphics rendering working.
- [x] Get audio playback working.
- [x] Run the full game on Xbox Series S.
- [x] Controller pointer and menu navigation.
- [x] Choice of left or right stick for the pointer.
- [ ] Polish controller-based gameplay.
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
