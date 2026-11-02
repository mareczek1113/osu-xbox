using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UwpOsu;

sealed partial class App : Application
{
    /// <summary>
    /// How the osu! cursor is driven, from pointer.txt in the app's local folder (written with "hybrid" on first run):
    /// <list type="bullet">
    /// <item><b>hybrid</b>: the console pointer stays on but is hidden. The pad moves the cursor itself (left stick, A = click) and a USB mouse moves it from raw mouse movement.</item>
    /// <item><b>pad</b>: the console pointer is off, only the pad drives the cursor. A USB mouse does not work in this mode.</item>
    /// <item><b>system</b>: the console's own pointer (left stick + A) is used, shown on top of osu!'s cursor.</item>
    /// </list>
    /// </summary>
    public static string PointerMode { get; private set; } = "hybrid";

    public App()
    {
        InitializeComponent();

        try
        {
            string path = System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "pointer.txt");

            if (System.IO.File.Exists(path))
            {
                string mode = System.IO.File.ReadAllText(path).Trim().ToLowerInvariant();

                if (mode == "pad" || mode == "system" || mode == "hybrid")
                    PointerMode = mode;
            }
            else
                System.IO.File.WriteAllText(path, "hybrid\n");
        }
        catch
        {
            // Keep the default.
        }

        // Only "pad" switches the console's pointer off; with it off the console also stops delivering USB mouse input.
        RequiresPointerMode = PointerMode == "pad" ? ApplicationRequiresPointerMode.WhenRequested : ApplicationRequiresPointerMode.Auto;
    }

    protected override void OnLaunched(LaunchActivatedEventArgs e)
    {
        if (Window.Current.Content is not Frame)
            Window.Current.Content = new Frame();

        var frame = (Frame)Window.Current.Content;

        if (frame.Content == null)
            frame.Navigate(typeof(MainPage), e.Arguments);

        Window.Current.Activate();
    }
}
