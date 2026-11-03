using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using osu.Framework.Input.Handlers;
using osu.Framework.Input.Handlers.Joystick;
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

            default:
                return base.CreateSettingsSubsectionFor(handler);
        }
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        log("osu.Game LoadComplete");
        startImportWatcher();
    }

    // Beatmaps (.osz), skins (.osk), replays (.osr) and legacy collections' packs (.olz) dropped into the app's LocalState\import folder
    // (Device Portal > File explorer > LocalState) are imported automatically.
    private static readonly string[] import_extensions = { ".osz", ".osk", ".osr", ".olz" };

    private string importFolder = string.Empty;
    private volatile bool importing;

    private void startImportWatcher()
    {
        try
        {
            importFolder = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "import");
            Directory.CreateDirectory(importFolder);
            Directory.CreateDirectory(Path.Combine(importFolder, "done"));
            log("import folder: " + importFolder);
        }
        catch (Exception e)
        {
            log("import folder setup failed: " + e.Message);
            return;
        }

        Scheduler.AddDelayed(scanImportFolder, 3000, true);
    }

    private void scanImportFolder()
    {
        if (importing)
            return;

        string[] ready;

        try
        {
            // A file still being uploaded is skipped: it must be older than 3 seconds and openable for exclusive reading.
            ready = Directory.EnumerateFiles(importFolder)
                             .Where(f => import_extensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
                             .Where(isFinished)
                             .ToArray();
        }
        catch (Exception e)
        {
            log("import scan failed: " + e.Message);
            return;
        }

        if (ready.Length == 0)
            return;

        importing = true;
        log($"importing {ready.Length} file(s) from the import folder");

        Task.Run(async () =>
        {
            try
            {
                await Import(ready).ConfigureAwait(false);

                // osu! deletes a file after importing it successfully. Anything still here was not imported, so park it in "done"
                // (otherwise it would be retried every 3 seconds forever).
                foreach (string file in ready)
                {
                    if (!File.Exists(file))
                        continue;

                    log("not consumed by the importer, moving to done: " + Path.GetFileName(file));
                    string target = Path.Combine(importFolder, "done", Path.GetFileName(file));
                    if (File.Exists(target)) File.Delete(target);
                    File.Move(file, target);
                }

                log("import finished");
            }
            catch (Exception e)
            {
                log("import failed: " + e);
            }
            finally
            {
                importing = false;
            }
        });
    }

    private static bool isFinished(string path)
    {
        try
        {
            if (DateTime.UtcNow - File.GetLastWriteTimeUtc(path) < TimeSpan.FromSeconds(3))
                return false;

            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
            return true;
        }
        catch (IOException)
        {
            return false;
        }
    }
}
