using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._RW.Skin;

/// <summary>
///     Component for the skin case items.
///     When used on matching items, applies the skin and consumes the case.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class ItemSkinCaseComponent : Component
{
    /// <summary>
    ///     How many times the skin case can be used before it is consumed.
    /// </summary>
    [DataField, AutoNetworkedField, ViewVariables(VVAccess.ReadWrite)]
    public int Uses = 1;

    /// <summary>
    ///     The entity prototype ID that this case can be applied to.
    /// </summary>
    [DataField(required: true), AutoNetworkedField]
    public EntProtoId TargetPrototype;

    /// <summary>
    ///     Ground/world sprite RSI path.
    /// </summary>
    [DataField(required: true), AutoNetworkedField]
    public string SpriteRsi = string.Empty;

    /// <summary>
    ///     Ground/world sprite state.
    /// </summary>
    [DataField, AutoNetworkedField]
    public string? SpriteState;

    /// <summary>
    ///     Optional in-hand visuals RSI path override.
    /// </summary>
    [DataField, AutoNetworkedField]
    public string? InhandRsi;

    /// <summary>
    ///     Optional clothing visuals RSI path override.
    /// </summary>
    [DataField, AutoNetworkedField]
    public string? ClothingRsi;

    /// <summary>
    ///     Sound played when the skin is successfully applied.
    /// </summary>
    [DataField]
    public SoundSpecifier ApplySound = new SoundPathSpecifier("/Audio/Effects/unwrap.ogg");
}
