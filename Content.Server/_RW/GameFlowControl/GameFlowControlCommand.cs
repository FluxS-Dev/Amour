using Content.Server.Administration;
using Content.Server.EUI;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server._RW.GameFlowControl;

[AdminCommand(AdminFlags.Spawn)]
public sealed class GameFlowControlCommand : IConsoleCommand
{
    public string Command => "gameflowcontrol";
    public string Description => "Opens the game flow control mode panel.";
    public string Help => "gameflowcontrol";

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (shell.Player is not { } player)
        {
            shell.WriteError(Loc.GetString("shell-cannot-run-command-from-server"));
            return;
        }

        var system = IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<GameFlowControlSystem>();
        if (system.IsOccupied() && system.GetOccupierName() != player.Name)
        {
            shell.WriteError(Loc.GetString("game-flow-control-error-occupied", ("username", system.GetOccupierName() ?? "")));
            return;
        }

        var euiManager = IoCManager.Resolve<EuiManager>();
        euiManager.OpenEui(new GameFlowControlEui(), player);
    }
}
