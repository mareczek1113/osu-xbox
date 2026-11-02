using System;
using osu.Framework.Input.Handlers;
using osu.Framework.Input.Handlers.Joystick;
using osu.Framework.Input.Handlers.Mouse;
using osu.Game;
using osu.Game.Overlays.Settings;

namespace UwpOsu;

/// <summary>
/// The osu! game for the Xbox UWP host. For now a plain <see cref="OsuGame"/>; console-specific overrides go here.
/// </summary>
public partial class XboxOsuGame : OsuGame
{
    private readonly Action<string> log;

    public XboxOsuGame(Action<string> log)
        : base()
    {
        this.log = log;

        // Native AOT cannot create mod-setting enum dropdowns by MakeGenericType; instantiate them statically (generated file).
        XboxEnumDropdowns.Register();
    }

    public override SettingsSubsection CreateSettingsSubsectionFor(InputHandler handler)
    {
        switch (handler)
        {
            case UwpGamepadHandler pad:
                return new XboxPadSettings(pad);

            case UwpMouseHandler mouse:
                return new XboxMouseSettings(mouse);

            default:
                return base.CreateSettingsSubsectionFor(handler);
        }
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        log("osu.Game LoadComplete");
    }
}
