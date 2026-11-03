<div align="center">

<img src="https://github.com/ppy/osu/blob/master/assets/lazer.png" width="100" alt="osu! logo">

# osu! Xbox

### Bringing osu!lazer to Xbox Series X|S.

An unofficial port bringing osu!lazer to Xbox Series X|S using UWP technologies.

<br>

![Status](https://img.shields.io/badge/STATUS-FIRST_PUBLIC_RELEASE-ff66aa?style=for-the-badge)
![Platform](https://img.shields.io/badge/PLATFORM-XBOX_SERIES_X%7CS-ff66aa?style=for-the-badge)
![Language](https://img.shields.io/badge/LANGUAGE-C%23-ff66aa?style=for-the-badge)
![Project](https://img.shields.io/badge/PROJECT-OSU!LAZER-ff66aa?style=for-the-badge)

<br>

**The first public release is now available!**

The full game is running on Xbox Series S, with graphics, audio, controller pointer support, menu navigation, controller-based gameplay, beatmap importing and skin importing working.

**[Download the latest release](https://github.com/mareczek1113/osu-xbox/releases)**

</div>

---

## **About**

osu! Xbox is an independent project bringing [osu!lazer](https://github.com/ppy/osu) to Xbox Series X|S.

The project adapts osu!'s existing codebase and framework to work within an Xbox environment using UWP.

The full game is running on Xbox Series S, with graphics rendering and audio playback working.

Xbox Series X is also a target platform, with compatibility testing still to be completed.

Beatmap and skin importing are working through Xbox Device Portal.

**Current milestone:** The first public release is available. osu!lazer runs on Xbox Series S with graphics, audio, controller input, beatmap importing and skin importing working.

## **Technology**

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

## **Development Status**

| Component | Status |
|---|---|
| Xbox application startup | Working |
| Full osu!lazer integration | Working |
| Graphics rendering | Working |
| Audio playback | Working |
| Xbox platform integration | Working |
| Framework adaptation | Working |
| Controller pointer and menus | Working |
| Controller-based gameplay | Working |
| Beatmap importing | Working |
| Skin importing | Working |
| First public release | Available |
| Xbox Series X testing | Not yet verified |

**Status key**

- **Working:** Feature is functional.
- **In progress:** Further development or testing is needed.

## **Current Focus: Controller Support**

With the full game running on Xbox and graphics and audio operational, development is focused on polishing controller input.

### **Controller pointer and menus**

With a controller, the Xbox acts as a mouse-driven osu! machine:

| Input | Action |
|---|---|
| Left stick (default) | Move the cursor |
| Right stick (default) | Scroll menus |
| A | Left click |
| Left trigger (held) | Precision mode, slows the cursor down |
| Menu button | Pause while playing, back in menus (can be switched off) |
| View button | Open the on-screen keyboard when a text box is active |

Open **Settings → Input → Gamepad (UWP)** to configure controller input.

- **Move pointer with right stick:** Swaps the sticks, so the right stick moves the cursor and the left stick scrolls.
- **Pointer speed, response curve and precision speed:** Adjust cursor movement.
- **Menu button pauses:** Also sends Escape, which pauses a beatmap and goes back in menus.
- **Menu scroll speed and stick deadzone:** Adjust menu scrolling and stick sensitivity.

### **Mouse support limitation**

Native mouse support is not currently available in this port.

The current implementation uses UWP-specific input handling. The way input is currently implemented does not provide the mouse functionality needed by osu!lazer.

Adding mouse support would require a different input implementation and additional platform-specific work.

For now, controller input is the primary way to navigate and play osu! Xbox.

### **Controller-based gameplay**

Controller-based gameplay is working. Further development focuses on polishing input behaviour and ensuring a consistent gameplay experience.

## **Technical Development**

### **Graphics**

The Xbox graphics implementation is working, allowing osu!lazer to render on Xbox Series S.

### **Audio**

Audio playback is working on Xbox, allowing the game to produce sound through its adapted audio implementation.

### **Platform compatibility**

The application and adapted framework are running in the Xbox environment using UWP.

The project targets Xbox Series X|S, with Series X testing still to be completed.

### **Input**

Controller pointer movement, menu navigation and controller-based gameplay are working.

Development is focused on polishing the remaining input behaviour.

Native mouse support is not currently implemented because of the limitations of the existing UWP-specific input implementation.

### **Beatmaps and skins**

Beatmaps and skins can be imported through Xbox Device Portal.

Supported import file types:

- `.osz` — Beatmap packages.
- `.osk` — Skin packages.

See the importing guide below for instructions.

## **Installation**

**[Download osu! Xbox from the GitHub Releases page](https://github.com/mareczek1113/osu-xbox/releases)**

The first public release is available. Download the installation package attached to the latest release.

### **Installation guide**

**1. Set up Xbox Developer Mode**

Enable Developer Mode on your Xbox Series X|S.

Follow this tutorial for help:

[Watch the Xbox Developer Mode setup tutorial](https://www.youtube.com/watch?v=ABdUwiBK1s0)

**2. Download the release**

- Open the [osu! Xbox Releases page](https://github.com/mareczek1113/osu-xbox/releases).
- Open the latest release.
- Download the installation package attached to it.

**3. Install osu! Xbox**

- Open Xbox Device Portal in a browser on your PC.
- Connect to your Xbox.
- Upload the downloaded installation package through Xbox Device Portal.
- Install the package on your Xbox.

**4. Launch osu! Xbox**

Once installed, launch osu! Xbox from your Xbox Developer Mode environment.

## **Importing Beatmaps and Skins**

You can import your own beatmaps and skins using Xbox Device Portal without needing to connect a USB drive.

### **1. Open Xbox Device Portal**

- Make sure your Xbox is running in Developer Mode.
- Open Xbox Device Portal on your PC.
- Connect to your Xbox.

### **2. Open the game's Local State folder**

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
