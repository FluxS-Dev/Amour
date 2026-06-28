using Content.Shared.DeviceLinking;
using Content.Shared.Mobs;
using Content.Shared._Shitmed.Medical.Surgery.Tools;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._RW.LifeTrigger;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class LifeTriggerComponent : Component, ISurgeryToolComponent
{
    [DataField, AutoNetworkedField]
    public MobState TriggerState = MobState.Dead;

    /// <summary>
    /// The mob state at which this trigger last fired. Null if not currently triggered.
    /// </summary>
    [DataField]
    public MobState? LastTriggeredState;

    [DataField]
    public ProtoId<SourcePortPrototype> Port = "LifeTriggered";

    public string ToolName => "life trigger";

    [DataField, AutoNetworkedField]
    public bool? Used { get; set; } = false;

    [DataField, AutoNetworkedField]
    public float Speed { get; set; } = 1f;
}
