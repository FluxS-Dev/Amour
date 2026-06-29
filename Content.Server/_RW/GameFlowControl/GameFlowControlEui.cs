using Content.Server.EUI;
using Content.Server.Administration.Managers;
using Content.Shared.Administration;
using Content.Shared.Eui;
using Content.Shared._RW.GameFlowControl;

namespace Content.Server._RW.GameFlowControl;

public sealed class GameFlowControlEui : BaseEui
{
    [Dependency] private readonly IEntityManager _entManager = default!;
    [Dependency] private readonly IAdminManager _adminManager = default!;

    private readonly GameFlowControlSystem _gameFlowControl;

    public GameFlowControlEui()
    {
        IoCManager.InjectDependencies(this);
        _gameFlowControl = _entManager.System<GameFlowControlSystem>();
    }

    public override void Opened()
    {
        base.Opened();

        // Security check: Only allow admins with Spawn flag
        if (!_adminManager.HasAdminFlag(Player, AdminFlags.Spawn))
        {
            Close();
            return;
        }

        // Try to occupy the panel
        if (!_gameFlowControl.TryOccupy(Player))
        {
            Close();
            return;
        }

        _gameFlowControl.ActiveEui = this;
        StateDirty();
    }

    public override void Closed()
    {
        base.Closed();
        _gameFlowControl.ReleaseControl(Player);
    }

    public override EuiStateBase GetNewState()
    {
        var state = new GameFlowControlEuiState();
        _gameFlowControl.PopulateEuiState(state);
        return state;
    }

    public override void HandleMessage(EuiMessageBase msg)
    {
        base.HandleMessage(msg);

        // Security check again for any incoming control messages
        if (!_adminManager.HasAdminFlag(Player, AdminFlags.Spawn))
            return;

        switch (msg)
        {
            case SetIntervalMsg intervalMsg:
                _gameFlowControl.SetInterval(intervalMsg.Min, intervalMsg.Max, intervalMsg.TimeLeft);
                break;
            case SetChaosMsg chaosMsg:
                _gameFlowControl.SetChaos(chaosMsg.Score);
                break;
            case ApproveRuleMsg approveMsg:
                if (_entManager.TryGetEntity(approveMsg.Entity, out var approveUid))
                {
                    _gameFlowControl.ApproveRule(approveUid.Value);
                }
                break;
            case DenyRuleMsg denyMsg:
                if (_entManager.TryGetEntity(denyMsg.Entity, out var denyUid))
                {
                    _gameFlowControl.DenyRule(denyUid.Value);
                }
                break;
            case TriggerRuleMsg triggerMsg:
                _gameFlowControl.TriggerRule(triggerMsg.RuleId);
                break;
            case ReleaseControlMsg _:
                Close();
                break;
        }
    }
}
