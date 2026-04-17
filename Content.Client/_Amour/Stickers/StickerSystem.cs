using System.Collections.Generic;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Content.Shared._Amour.Stickers;

namespace Content.Client._Amour.Stickers;

public sealed class StickerSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    public IEnumerable<StickerPrototype> GetStickers()
    {
        return _prototypeManager.EnumeratePrototypes<StickerPrototype>();
    }
}
