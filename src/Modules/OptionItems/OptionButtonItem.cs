using BetterAmongUs.Generated;
using BetterAmongUs.Utilities;
using BetterAmongUs.Utilities.Extension;

namespace BetterAmongUs.Modules.OptionItems;

internal sealed class OptionButtonItem : OptionItem<bool>
{
    internal override bool CanLoad => false;

    private Action? onClick;

    internal static OptionButtonItem Create(OptionTab tab, TranslationStrings.TranslationString translationString, Action onClick)
    {
        if (GetOptionByTranslationName(translationString) is OptionButtonItem existing)
        {
            existing.onClick = onClick;
            existing.CreateBehavior();
            return existing;
        }

        OptionButtonItem Item = new();
        AllOptions.Add(Item);
        Item.Tab = tab;
        Item.TranslationName = translationString;
        Item.DefaultValue = false;
        Item.onClick = onClick;
        Item.CreateBehavior();
        return Item;
    }

    protected override void CreateBehavior()
    {
        if (!GameSettingMenu.Instance)
            return;

        AllOptionsTemp.Add(this);
        var toggleOption = UnityEngine.Object.Instantiate(Tab.AUTab.checkboxOrigin, Tab.AUTab.settingsContainer);
        Option = toggleOption;
        Obj = Option.gameObject;
        Option.enabled = false;
        Tab.Children.Add(this);
        TitleTMP = toggleOption.TitleText;
        SetupText(toggleOption.TitleText);
        SetupOptionBehavior();
        SetOptionVisuals();
    }

    protected override void SetupOptionBehavior()
    {
        if (Option is ToggleOption toggleOption)
        {
            SetupAUOption(Option);
            toggleOption.DestroyTextTranslators();
            toggleOption.TitleText.text = Name;
            toggleOption.CheckMark.enabled = false;
            var button = toggleOption.buttons[0];
            button.OnClick = new();
            button.OnClick.AddListener(() =>
            {
                if (onClick != null) onClick();
                toggleOption.CheckMark.enabled = false;
            });
        }
    }

    internal override void UpdateVisuals(bool updateTabVisuals = true)
    {
        if (Option is ToggleOption toggleOption)
        {
            toggleOption.CheckMark.enabled = false;
        }
        if (updateTabVisuals)
        {
            Tab.UpdateVisuals();
        }
    }

    public override string ValueAsString() => string.Empty;
    public override bool Is(bool value) => false;
}