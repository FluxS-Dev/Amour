using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._Amour.Stickers;

[Prototype("sticker")]
public sealed class StickerPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; } = default!;

    [DataField("texture")]
    public ResPath TexturePath { get; private set; } = default!;
}
