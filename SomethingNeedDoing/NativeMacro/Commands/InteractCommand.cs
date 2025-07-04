﻿using System.Threading;
using System.Threading.Tasks;

namespace SomethingNeedDoing.NativeMacro.Commands;
/// <summary>
/// Interacts with the current target.
/// </summary>
[GenericDoc(
    "Interact with the current target",
    [],
    ["/interact", "/interact <errorif.targetnotfound>"]
)]
public class InteractCommand(string text) : MacroCommandBase(text)
{
    /// <inheritdoc/>
    public override bool RequiresFrameworkThread => true;

    /// <inheritdoc/>
    public override async Task Execute(MacroContext context, CancellationToken token)
    {
        await context.RunOnFramework(() =>
        {
            if (Game.Interact(Svc.Targets.Target))
                Svc.Log.Debug($"Interacting with [{Svc.Targets.Target?.Address:X}] {Svc.Targets.Target?.Name}");
            else
                Svc.Log.Warning($"Failed to interact with target.");
        });

        await PerformWait(token);
    }
}
