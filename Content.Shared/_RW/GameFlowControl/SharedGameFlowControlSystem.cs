using Robust.Shared.Serialization;

namespace Content.Shared._RW.GameFlowControl;

/// <summary>
///     Shared base system for managing Game Flow Control.
/// </summary>
public abstract class SharedGameFlowControlSystem : EntitySystem
{
}

/// <summary>
///     Added to game rules that are pending admin approval.
/// </summary>
[RegisterComponent]
public sealed partial class PendingApprovalRuleComponent : Component
{
    [ViewVariables]
    public TimeSpan Timeout;

    [ViewVariables]
    public bool IsStationEvent;
}

/// <summary>
///     Marker component to indicate that this rule has been approved and should skip intercept checks.
/// </summary>
[RegisterComponent]
public sealed partial class GameFlowControlApprovedComponent : Component
{
}

/// <summary>
///     Sent by the client when they need to request the initial or current occupier state.
/// </summary>
[Serializable, NetSerializable]
public sealed class RequestGameFlowControlStateEvent : EntityEventArgs
{
}

/// <summary>
///     Broadcasted to update client admin menu button states and EUI availability.
/// </summary>
[Serializable, NetSerializable]
public sealed class GameFlowControlStateEvent : EntityEventArgs
{
    public string? OccupierName { get; }

    public GameFlowControlStateEvent(string? occupierName)
    {
        OccupierName = occupierName;
    }
}

/// <summary>
///     Raised as a directed by-ref event on game rule entities when they are approved
///     to handle delayed announcements/audio.
/// </summary>
[ByRefEvent]
public struct GameFlowControlRuleApprovedEvent
{
}
