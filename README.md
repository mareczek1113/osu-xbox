 <div align="center">

<img src="https://github.com/ppy/osu/blob/master/assets/lazer.png" width="100" alt="osu! logo">

# osu! Xbox

### Bringing osu!lazer to Xbox Series X|S.

An unofficial port bringing osu!lazer to Xbox Series X|S using UWP technologies.

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

The project adapts osu!'s existing codebase and framework to work within an Xbox environment using UWP.

The full game is running on Xbox Series S, with graphics rendering and audio playback working.

Xbox Series X is also a target platform, with compatibility testing still to be completed.

Beatmap and skin importing are working through Xbox Device Portal.

> **Current milestone:** osu!lazer runs on Xbox Series S. Graphics, audio, controller pointer navigation, menus, controller-based gameplay, beatmap importing and skin importing are working.

## ✨ Goals

- 🎮 Polish Xbox controller support.
- 🎵 Maintain reliable audio playback and timing.
- 🎨 Keep graphics rendering working across Xbox hardware.
- 🗺️ Support beatmap importing.
- 🎨 Support skin importing.
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
| Platform technology | UWP |
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
| Controller-based gameplay | 🟢 Working |
| Beatmap importing | 🟢 Working |
| Skin importing | 🟢 Working |
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

Open **Settings → Input → Gamepad (UWP)** to tune it:

- **Move pointer with right stick**: swaps the sticks, so the right stick moves the cursor and the left stick scrolls.
- **Pointer speed**, **response curve** and **precision speed**.
- **Menu button pauses**: also sends Escape, which pauses a beatmap and goes back in menus.
- **Menu scroll speed** and **stick deadzone**.

### Mouse support limitation

Native mouse support is not currently available in this port.

The current implementation uses UWP-specific input handling, which has limitations in the Xbox environment. The way input is currently implemented does not provide the mouse functionality needed by osu!lazer.

Adding mouse support would require a different input implementation and additional platform-specific work.

For now, controller input is the primary way to navigate and play osu! Xbox.

### Controller-based gameplay

Controller-based gameplay is working. The remaining work focuses on polishing input behaviour and ensuring a consistent gameplay experience.

## 🧪 Technical Development

### Graphics

The Xbox graphics implementation is working, allowing osu!lazer to render on Xbox Series S.

### Audio

Audio playback is working on Xbox, allowing the game to produce sound through its adapted audio implementation.

### Platform compatibility

The application and adapted framework are running in the Xbox environment using UWP.

The project targets Xbox Series X|S, with Series X testing still to be completed.

### Input

Controller pointer movement, menu navigation and controller-based gameplay are working. Development is focused on polishing the remaining input behaviour.

Native mouse support is not currently implemented because of the limitations of the existing UWP-specific input implementation.

### Beatmaps and skins

Beatmaps and skins can be imported through Xbox Device Portal.

Supported import file types:

- `.osz` — Beatmap packages.
- `.osk` — Skin packages.

See the installation and importing guides below for instructions.

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
- Open Xbox Device Portal in a browser on your PC.
- Upload the installation package through Xbox Device Portal.
- Install the package on your Xbox.

<text color="secondary" size="xs">The installation package and download link will be added when the first public release is ready.</text>

**3. Configure the application**

Make sure the UWP application is configured correctly for Xbox deployment.

**4. Launch osu! Xbox**

Once installed and configured, launch osu! Xbox from your Xbox Developer Mode environment.

## 🗺️ Importing Beatmaps and Skins

You can import your own beatmaps and skins using Xbox Device Portal without needing to connect a USB drive.

### 1. Open Xbox Device Portal

- Make sure your Xbox is running in Developer Mode.
- Open Xbox Device Portal on your PC.
- Connect to your Xbox.

### 2. Open the game's Local State folder

In Xbox Device Portal:

1. Open **File Explorer**.
2. Navigate to **LocalAppData**.
3. Find and open **osu! Xbox**.
4. Open **LocalState**.
5. Open the **Import** folder.

The folder path is:

```text
LocalAppData
└── osu! Xbox
    └── LocalState
        └── Import
```

### 3. Upload your beatmaps or skins

Upload your files directly into the `Import` folder.

| File extension | Content |
|---|---|
| `.osz` | Beatmap packages |
| `.osk` | Skin packages |

**Beatmaps:** Upload your `.osz` files.

**Skins:** Upload your `.osk` files.

### 4. Import into osu! Xbox

After uploading your files, open osu! Xbox and allow the game to process the imported content.

Your imported beatmaps and skins should then be available in the game.

<text color="secondary" size="xs">Make sure you upload the original `.osz` or `.osk` package files rather than extracting them first.</text>

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
- [x] Controller-based gameplay.
- [x] Beatmap importing.
- [x] Skin importing.
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
