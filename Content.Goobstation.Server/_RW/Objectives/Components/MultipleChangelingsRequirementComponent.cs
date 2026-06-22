using Content.Goobstation.Server._RW.Objectives.Systems;

namespace Content.Goobstation.Server._RW.Objectives.Components;

[RegisterComponent, Access(typeof(MultipleChangelingsRequirementSystem))]
public sealed partial class MultipleChangelingsRequirementComponent : Component
{
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public int Changelings = 1;
}
