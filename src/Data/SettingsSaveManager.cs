using BetterAmongUs.Data.Json;
using BetterAmongUs.Managers;
using BetterAmongUs.Modules;
using BetterAmongUs.Modules.OptionItems;
using BetterAmongUs.Patches.Gameplay.UI.Settings;

namespace BetterAmongUs.Data;

internal static class SettingsSaveManager
{
    private const string VanillaKey = "Vanilla.Options";
    private const string GameModeKey = "Vanilla.GameMode";

    private static string SavesFolder => Path.Combine(BetterDataManager.Folders.settingsFolderPath, "Saves");

    internal static string[] GetSaveFiles()
    {
        try
        {
            if (!Directory.Exists(SavesFolder)) return [];
            var files = Directory.GetFiles(SavesFolder, "*.json");
            Array.Sort(files, StringComparer.OrdinalIgnoreCase);
            return files;
        }
        catch { return []; }
    }

    internal static void Save()
    {
        try
        {
            Directory.CreateDirectory(SavesFolder);
            string path = Path.Combine(SavesFolder, $"Save-{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.json");

            var file = new BetterGameSettingsFile { OverrideFilePath = path };
            foreach (var kvp in BetterDataManager.Files.BetterGameSettingsFile.Settings)
            {
                if (kvp.Key == BetterGameSettingsFile.SETTINGS_VERSION_KEY) continue;
                file.Settings[kvp.Key] = kvp.Value;
            }

            var manager = GameOptionsManager.Instance;
            if (manager != null && manager.CurrentGameOptions != null)
            {
                var bytes = manager.gameOptionsFactory.ToBytes(manager.CurrentGameOptions, false);
                file.Settings[VanillaKey] = Convert.ToBase64String(bytes.ToArray());
                file.Settings[GameModeKey] = (int)manager.currentGameMode;
            }

            file.Save();
            var selector = OptionSaveFileItem.Instance;
            if (selector != null)
            {
                selector.Refresh(path);
                selector.UpdateVisuals(false);
            }
            BetterNotificationManager.Notify($"Saved: {Path.GetFileNameWithoutExtension(path)}", 2.5f);
        }
        catch (Exception ex)
        {
            BAUPlugin.Logger.Error(ex);
            BetterNotificationManager.Notify("Failed to save settings!", 2.5f);
        }
    }

    internal static void Load()
    {
        var selector = OptionSaveFileItem.Instance;
        string? path = selector != null ? selector.SelectedFile : null;
        if (path == null)
        {
            BetterNotificationManager.Notify("No saved settings found!", 2.5f);
            return;
        }

        try
        {
            var file = new BetterGameSettingsFile { OverrideFilePath = path };
            file.Init();

            // Better settings
            var current = BetterDataManager.Files.BetterGameSettingsFile;
            foreach (var kvp in file.Settings)
            {
                if (kvp.Key == BetterGameSettingsFile.SETTINGS_VERSION_KEY || kvp.Key == VanillaKey || kvp.Key == GameModeKey) continue;
                current.Settings[kvp.Key] = kvp.Value;
            }
            current.Save();
            foreach (var opt in OptionItem.AllOptions)
            {
                opt.TryLoad(true);
            }
            GameSettingsPatch.BetterSettingsTab.UpdateVisuals();

            // Vanilla settings (host in lobby only)
            string message = "Better settings loaded!";
            if (file.Settings.TryGetValue(VanillaKey, out var raw) && raw is string base64)
            {
                if (GameState.IsHost && GameState.IsLobby)
                {
                    var manager = GameOptionsManager.Instance;
                    bool sameMode = !file.Settings.TryGetValue(GameModeKey, out var mode) || mode is not int modeInt || modeInt == (int)manager.currentGameMode;
                    if (sameMode)
                    {
                        var opts = manager.gameOptionsFactory.FromBytes(Convert.FromBase64String(base64));
                        manager.CurrentGameOptions = opts;
                        GameManager.Instance.LogicOptions.SetGameOptions(opts);
                        GameManager.Instance.LogicOptions.SyncOptions();
                        message = "Settings loaded!";
                    }
                    else message = "Better settings loaded (other game mode, vanilla skipped)";
                }
                else message = "Better settings loaded (vanilla: host in lobby only)";
            }
            BetterNotificationManager.Notify(message, 2.5f);
        }
        catch (Exception ex)
        {
            BAUPlugin.Logger.Error(ex);
            BetterNotificationManager.Notify("Failed to load settings!", 2.5f);
        }
    }
}