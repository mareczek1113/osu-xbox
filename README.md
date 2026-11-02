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

**The full game is running on Xbox Series S, with graphics and audio working. Mouse and pointer input, Xbox controller support and controller-based gameplay are almost complete.**

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
| Mouse and pointer input | 🟡 Almost complete |
| Xbox controller support | 🟡 Almost complete |
| Controller-based gameplay | 🟡 Almost complete |
| Beatmap support | 🔴 Not started |
| Skin support | 🔴 Not started |
| Xbox Series X testing | 🟡 Not yet verified |

**Status key**

- 🟢 Working
- 🟡 Almost complete / in progress
- 🔴 Not started / not completed

## 🎮 Current Focus: Input Support

With the full game running on Xbox and graphics and audio operational, development is focused on completing input support.

### Mouse and pointer

Mouse and pointer input is almost complete. The remaining work focuses on making pointer behaviour consistent and reliable within osu!lazer.

### Xbox controller

Xbox controller support is almost complete.

The goal is to provide reliable controller input throughout the game, including:

- Navigation through menus.
- Selecting and confirming interface elements.
- Navigating song selection.
- Gameplay input.
- Consistent input handling across the game.

### Controller-based gameplay

Controller-based gameplay is also almost complete, with the remaining work focused on polishing and completing the gameplay input experience.

## 🧪 Technical Development

### Graphics

The Xbox graphics implementation is working, allowing osu!lazer to render on Xbox Series S.

### Audio

Audio playback is working on Xbox, allowing the game to produce sound through its adapted audio implementation.

### Platform compatibility

The application and adapted framework are running in the Xbox environment.

The project targets Xbox Series X|S, with Series X testing still to be completed.

### Input

Mouse and pointer input, Xbox controller support and controller-based gameplay are almost complete.

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
- [ ] Finish mouse and pointer input.
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
