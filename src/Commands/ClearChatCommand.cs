using BetterAmongUs.Attributes;
using BetterAmongUs.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterAmongUs.Commands;

[RegisterCommand]
internal class ClearChatCommand : BaseCommand
{
    internal override string Name => "clearchat";
    internal override string Description => "Clears chat";

    internal override void Run()
    {
        Utils.ClearChat();
    }
}

