using BetterAmongUs.Generated;
using BetterAmongUs.Data;
using BetterAmongUs.Modules;
using BetterAmongUs.Patches.Gameplay.UI.Settings;

namespace BetterAmongUs.Modules.OptionItems;

internal sealed class OptionSaveFileItem : OptionStringItem
{
    internal override bool CanLoad => false;

    internal static OptionSaveFileItem? Instance { get; private set; }

    private string[] files = [];

    internal string? SelectedFile => files.Length > 0 && Value >= 0 && Value < files.Length ? files[Value] : null;

    internal static OptionSaveFileItem Create()
    {
        if (GetOptionByTranslationName(TranslationStrings.BetterSetting_Setting_SaveFile) is OptionSaveFileItem existing)
        {
            Instance = existing;
            existing.Refresh(null);
            existing.CreateBehavior();
            return existing;
        }

        OptionSaveFileItem Item = new();
        AllOptions.Add(Item);
        Item.Tab = GameSettingsPatch.BetterSettingsTab;
        Item.TranslationName = TranslationStrings.BetterSetting_Setting_SaveFile;
        Item.TranslatorStrings = [new TranslationStrings.TranslationString(string.Empty)];
        Item.DefaultValue = 0;
        Item.Value = 0;
        Item.Refresh(null);
        Instance = Item;
        Item.CreateBehavior();
        return Item;
    }

    internal void Refresh(string? select)
    {
        files = SettingsSaveManager.GetSaveFiles();
        Range = new IntRange(0, Math.Max(0, files.Length - 1));
        int index = select == null ? -1 : Array.IndexOf(files, select);
        Value = index >= 0 ? index : Math.Clamp(Value, 0, Range.max);
    }

    public override string ValueAsString()
    {
        var file = SelectedFile;
        return file == null ? "-" : Path.GetFileNameWithoutExtension(file);
    }
}