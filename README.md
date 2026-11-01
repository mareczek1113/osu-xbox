 <div align="center">

<img src="https://github.com/ppy/osu/blob/master/assets/lazer.png" width="100" alt="osu! logo">

# osu! Xbox

### Bringing osu!lazer to Xbox Series S.

An unofficial port bringing osu!lazer to Xbox Series S using UWP/GDK technologies.

<br>

![Status](https://img.shields.io/badge/STATUS-WORKING_PORT-ff66aa?style=for-the-badge)
![Platform](https://img.shields.io/badge/PLATFORM-XBOX_SERIES_S-ff66aa?style=for-the-badge)
![Language](https://img.shields.io/badge/LANGUAGE-C%23-ff66aa?style=for-the-badge)
![Project](https://img.shields.io/badge/PROJECT-OSU!LAZER-ff66aa?style=for-the-badge)

<br>

**An independent project bringing the osu!lazer experience to Xbox. The game is running, with controller support being the main remaining development goal.**

</div>

---

## 💗 About

osu! Xbox is an independent project bringing [osu!lazer](https://github.com/ppy/osu) to Xbox Series S.

The project adapts osu!'s existing codebase and framework to work within an Xbox environment.

The full game is now running on Xbox, with graphics rendering and audio playback working.

The current focus is controller integration, with the goal of making osu! fully playable using an Xbox controller.

> **Current milestone:** osu!lazer runs on Xbox Series S. Controller support is still in development.

## ✨ Goals

- 🎮 Implement full Xbox controller support.
- 🎵 Maintain reliable audio playback and timing.
- 🎨 Keep graphics rendering working on Xbox hardware.
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
| Target platform | Xbox Series S |
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
| Mouse and pointer input | 🟡 Under investigation |
| Xbox controller support | 🟡 Main development focus |
| Full controller-based gameplay | 🔴 Not yet completed |

**Status key**

- 🟢 Working
- 🟡 In progress
- 🔴 Not completed

## 🎮 Current Focus: Controller Support

With the game running on Xbox and graphics and audio operational, development is now focused on Xbox controller integration.

The goal is to support:

- Navigation through menus.
- Selecting and confirming interface elements.
- Navigating song selection.
- Gameplay input.
- Consistent input handling across the game.

The existing mouse and pointer input behaviour is also being investigated to ensure input feels consistent on Xbox.

## 🧪 Technical Development

### Graphics

The Xbox graphics implementation is working, allowing osu!lazer to render on the console.

### Audio

Audio playback is working on Xbox, allowing the game to produce sound through its adapted audio implementation.

### Platform compatibility

The application and adapted framework are running in the Xbox environment.

### Input

Controller support is the main remaining milestone. Work is focused on connecting Xbox input to osu!'s existing input and gameplay systems.

## 📦 Installation

There are currently no public releases or installation packages.

Release and installation instructions will be added when a public build is ready.

## 🗺️ Roadmap

- [x] Create the project repository.
- [x] Adapt osu!framework for Xbox.
- [x] Get the Xbox application running.
- [x] Integrate osu!lazer.
- [x] Get graphics rendering working.
- [x] Get audio playback working.
- [x] Run the full game on Xbox Series S.
- [ ] Complete Xbox controller integration.
- [ ] Implement controller-based menu navigation.
- [ ] Implement controller-based gameplay.
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
