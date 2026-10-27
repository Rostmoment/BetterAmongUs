using BetterAmongUs.Data.Config;
using HarmonyLib;
using System.Globalization;
using UnityEngine;

namespace BetterAmongUs.Patches.Client;

[HarmonyPatch]
public static class ButtonCooldownPatch
{

    [HarmonyPatch(typeof(ActionButton), nameof(ActionButton.SetCoolDown))]
    [HarmonyPostfix]
    public static void Postfix(ActionButton __instance, ref float timer)
    {
        if (!__instance.isActiveAndEnabled || !BAUConfigs.DecimalCooldowns.Value)
            return;

        if (!__instance.isCoolingDown)
            return;

        var text = __instance.cooldownTimerText;
        if (text == null)
            return;

        // Расширяем один раз: если ширина уже достаточная, ничего не делаем
        var rect = text.rectTransform;
        if (rect.sizeDelta.x < BAUPlugin.Constants.MIN_TEXT_DECIMAL_COOLDOWN_WIDTH)
        {
            rect.sizeDelta = new Vector2(BAUPlugin.Constants.MIN_TEXT_DECIMAL_COOLDOWN_WIDTH, rect.sizeDelta.y);
            text.enableWordWrapping = false;
        }

        text.text = timer.ToString("0.00", CultureInfo.InvariantCulture);
    }
}