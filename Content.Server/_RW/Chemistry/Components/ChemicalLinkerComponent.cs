namespace Content.Server.Chemistry.Components
{
    [RegisterComponent]
    public sealed partial class ChemicalLinkerComponent : Component
    {
        [DataField]
        public EntityUid? SavedDevice;
    }
}
