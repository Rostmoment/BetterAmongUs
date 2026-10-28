using BetterAmongUs.Attributes;
using BetterAmongUs.Patches.Gameplay.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterAmongUs.Commands;

[RegisterCommand]
internal sealed class SummaryCommand : BaseCommand
{
    internal override string Name => "summary";

    internal override string Description => "Shows summary of last game";

    internal override bool CanRunCommand(out string reason)
    {
        reason = "";
        if (GameSummaryPatch.SummaryText.IsNullOrWhiteSpace())
        {
            reason = "Last game summary is empty";
            return false;
        }
        return true;
    }
    internal override void Run()
    {
        CommandResultText(GameSummaryPatch.SummaryText);
    }
}
