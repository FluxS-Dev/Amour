using Content.Goobstation.Server._RW.Objectives.Systems;

namespace Content.Goobstation.Server._RW.Objectives.Components;

[RegisterComponent, Access(typeof(ChangelingRandomChanceRequirementSystem))]
public sealed partial class ChangelingRandomChanceRequirementComponent : Component
{
    [DataField("chance"), ViewVariables(VVAccess.ReadWrite)]
    public float Chance = 0.3f;
}
