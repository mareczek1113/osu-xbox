<div align="center">

<img src="https://github.com/ppy/osu/blob/master/assets/lazer.png" width="100" alt="osu! logo">

# osu! Xbox

### Bringing osu!lazer to Xbox Series S.

An unofficial port exploring the feasibility of running
osu!lazer on Xbox using UWP/GDK technologies.

<br>

![Status](https://img.shields.io/badge/STATUS-EXPERIMENTAL-ff66aa?style=for-the-badge)
![Platform](https://img.shields.io/badge/PLATFORM-XBOX-ff66aa?style=for-the-badge)
![Language](https://img.shields.io/badge/LANGUAGE-C%23-ff66aa?style=for-the-badge)
![Project](https://img.shields.io/badge/PROJECT-OSU!LAZER-ff66aa?style=for-the-badge)

<br>

**An independent project made for the osu! community, exploring the technical challenges of bringing osu!lazer to Xbox.**

</div>

---

## 💗 About

osu! Xbox is an independent project developed for the osu! community, exploring the possibility of running
[osu!lazer](https://github.com/ppy/osu) on Xbox Series S.

The project investigates the compatibility of osu!'s existing codebase with
Xbox development technologies, including graphics, audio, input and platform
integration.

The aim is to preserve the core osu! experience while adapting the underlying
technology for Xbox hardware.

> This project is experimental. Console compatibility and a playable build
> have not yet been established.

## ✨ Goals

- 🎮 Investigate Xbox controller support.
- 🎵 Explore accurate audio timing and playback.
- 🎨 Investigate a compatible graphics rendering backend.
- ⚡ Evaluate performance on Xbox hardware.
- 🖥️ Explore adapting osu!'s interface for television displays.
- 🔧 Investigate .NET NativeAOT compatibility.
- 💗 Preserve the core osu! gameplay experience.

## 🛠️ Technology

| Component | Technology |
|---|---|
| Original game | osu!lazer |
| Main language | C# |
| Framework | osu!framework |
| Target platform | Xbox Series S |
| Platform technology | UWP/GDK investigation |
| Graphics | D3D12 investigation |
| Audio | SDL3 mixer prototype |
| Compilation | .NET NativeAOT investigation |

Technologies listed above are under investigation and may change during
development.

## 📊 Development Status

| Component | Status |
|---|---|
| Repository setup | 🟢 Started |
| Source code investigation | 🟢 Started |
| Audio mixer prototype | 🟡 Experimental |
| NativeAOT compatibility | 🟡 Under investigation |
| Xbox platform integration | 🔴 Not verified |
| Graphics backend | 🔴 Not implemented |
| Controller integration | 🔴 Not implemented |
| Full game integration | 🔴 Not implemented |
| Playable Xbox build | 🔴 Not available |

**Status key**

- 🟢 Started or available for investigation
- 🟡 Experimental or in progress
- 🔴 Not implemented or not verified

## 🧪 Research

Development is currently focused on investigating the main technical
challenges.

### Audio

A standalone software mixer prototype has been developed for experimentation
with playback speed, sample mixing and audio-position timing.

The prototype has not yet been integrated into osu!framework, and its results
do not establish actual Xbox audio latency.

### Graphics

The existing rendering architecture is being investigated to determine
what changes would be needed for an Xbox-compatible Direct3D 12 backend.

### Platform compatibility

The project is investigating .NET compilation, native dependencies,
input handling and platform-specific APIs.

## 📦 Installation

There are currently no public releases or playable Xbox builds.

Installation instructions will be added when a working build becomes
available.

## 🗺️ Roadmap

- [x] Create the project repository.
- [x] Begin investigating osu!lazer and osu!framework.
- [x] Develop an experimental audio mixer prototype.
- [ ] Validate the target Xbox development environment.
- [ ] Complete a minimal platform compatibility test.
- [ ] Investigate graphics backend integration.
- [ ] Integrate audio with osu!framework.
- [ ] Investigate controller support.
- [ ] Boot osu!lazer using the adapted platform components.
- [ ] Test performance and gameplay timing.
- [ ] Publish a build if feasible.

## 🤝 Contributing

Suggestions, technical discussion and contributions are welcome.

Please open an issue to discuss significant changes before submitting
a pull request.

All contributions must respect the applicable licences of the original
project and third-party dependencies.

## 💗 Credits

- [osu!](https://osu.ppy.sh/) — Original rhythm game.
- [ppy/osu](https://github.com/ppy/osu) — Open-source osu!lazer project.
- [ppy/osu-framework](https://github.com/ppy/osu-framework) — Game framework.

This project is independently developed for the osu! community and is not
affiliated with, endorsed by, or officially supported by ppy or the osu!
development team.

## 📄 Licence

The licensing terms for this port and its original components must be
respected.

See [LICENSE](LICENSE) for the licence applicable to this repository.

---

<div align="center">

**Made with 💗 for the osu! community.**

*osu! Xbox — an independent project by fans of osu!*

</div>
