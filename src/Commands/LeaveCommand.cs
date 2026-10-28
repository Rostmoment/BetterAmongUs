using BetterAmongUs.Attributes;
using BetterAmongUs.Modules;
using BetterAmongUs.Utilities;

namespace BetterAmongUs.Commands;

// Added in case if options menu doesn't work
[RegisterCommand]
internal sealed class LeaveCommand : BaseCommand
{
    internal override string Name => "leave";
    internal override string Description => "Leave the current lobby or game";

    internal override bool CanRunCommand(out string reason)
    {
        if (!GameState.IsInGame)
        {
            reason = "Can only run while in a lobby or game";
            return false;
        }

        return base.CanRunCommand(out reason);
    }

    internal override void Run()
    {
        Utils.DisconnectSelf("You left the game.", true);
    }
}