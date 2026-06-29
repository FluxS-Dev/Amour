using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared._RW.Skin;

/// <summary>
///     Component added to an item when a skin case has been applied.
///     Stores the custom visual paths that override the default ones.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class AppliedItemSkinComponent : Component
{
    [DataField, AutoNetworkedField]
    public string? SpriteRsi;

    [DataField, AutoNetworkedField]
    public string? SpriteState;

    [DataField, AutoNetworkedField]
    public string? InhandRsi;

    [DataField, AutoNetworkedField]
    public string? ClothingRsi;
}
