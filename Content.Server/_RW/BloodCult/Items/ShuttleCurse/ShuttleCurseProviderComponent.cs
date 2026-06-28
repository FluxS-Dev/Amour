namespace Content.Server._RW.BloodCult.Items.ShuttleCurse;

[RegisterComponent]
public sealed partial class ShuttleCurseProviderComponent : Component
{
    [DataField]
    public int MaxUses = 3;

    [ViewVariables(VVAccess.ReadOnly)]
    public int CurrentUses = 0;
}
