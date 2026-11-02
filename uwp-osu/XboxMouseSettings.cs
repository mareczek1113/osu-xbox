using osu.Framework.Graphics;
using osu.Framework.Input.Handlers.Mouse;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Overlays.Settings;
using osu.Game.Overlays.Settings.Sections;

namespace UwpOsu;

/// <summary>
/// Settings for a USB mouse (and any other pointer the console reports as a mouse).
/// </summary>
public partial class XboxMouseSettings : InputSubsection
{
    public XboxMouseSettings(UwpMouseHandler mouse)
        : base(mouse)
    {
        // Built in the constructor (no reflected load method) so Native AOT trimming cannot drop it.
        AddRange(new Drawable[]
        {
            new SettingsItemV2(new FormSliderBar<float>
            {
                Caption = "Mouse sensitivity",
                HintText = "Applies when the mouse moves the cursor from raw movement (pointer mode hybrid, the default).",
                KeyboardStep = 0.05f,
                Current = mouse.MouseSpeed.GetBoundCopy(),
            }),
        });
    }
}
