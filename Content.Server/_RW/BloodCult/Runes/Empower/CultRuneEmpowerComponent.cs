namespace Content.Server._RW.BloodCult.Runes.Empower;

[RegisterComponent]
public sealed partial class CultRuneEmpowerComponent : Component
{
    [DataField]
    public string ComponentToGive = "BloodCultEmpowered";
}
