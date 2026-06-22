namespace Content.Server.Chemistry.Components
{
    [RegisterComponent]
    public sealed partial class ChemicalLinkComponent : Component
    {
        [DataField]
        public EntityUid? LinkedDevice;
    }
}
