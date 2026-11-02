using osu.Framework.Graphics;
using osu.Framework.Input.Handlers.Joystick;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Overlays.Settings;
using osu.Game.Overlays.Settings.Sections;

namespace UwpOsu;

/// <summary>
/// Settings for the Xbox pad: pointer speed and response, precision mode, scroll speed and deadzone.
/// All of these are bindables on <see cref="UwpGamepadHandler"/>, so the framework saves them to input.json by itself.
/// </summary>
public partial class XboxPadSettings : InputSubsection
{
    public XboxPadSettings(UwpGamepadHandler pad)
        : base(pad)
    {
        // Built in the constructor (no reflected load method) so Native AOT trimming cannot drop it.
        AddRange(new Drawable[]
        {
            new SettingsItemV2(new FormSliderBar<float>
            {
                Caption = "Pointer speed",
                HintText = "How fast the left stick moves the cursor.",
                KeyboardStep = 0.05f,
                Current = pad.PointerSpeed.GetBoundCopy(),
            }),
            new SettingsItemV2(new FormSliderBar<float>
            {
                Caption = "Pointer response curve",
                HintText = "Higher values give finer control with small stick movements.",
                KeyboardStep = 0.1f,
                Current = pad.PointerCurve.GetBoundCopy(),
            }),
            new SettingsItemV2(new FormSliderBar<float>
            {
                Caption = "Precision speed (hold left trigger)",
                HintText = "Cursor speed multiplier while the left trigger is held.",
                KeyboardStep = 0.05f,
                Current = pad.PrecisionSpeed.GetBoundCopy(),
            }),
            new SettingsItemV2(new FormCheckBox
            {
                Caption = "Menu button pauses",
                HintText = "Also sends Escape, which pauses while playing a beatmap and goes back in menus.",
                Current = pad.MenuButtonPauses.GetBoundCopy(),
            }),
            new SettingsItemV2(new FormSliderBar<float>
            {
                Caption = "Menu scroll speed (right stick)",
                HintText = "Wheel notches per second at full tilt.",
                KeyboardStep = 0.5f,
                Current = pad.ScrollSpeed.GetBoundCopy(),
            }),
            new SettingsItemV2(new FormSliderBar<float>
            {
                Caption = "Stick deadzone",
                KeyboardStep = 0.01f,
                DisplayAsPercentage = true,
                Current = pad.DeadzoneThreshold.GetBoundCopy(),
            }),
        });
    }
}
