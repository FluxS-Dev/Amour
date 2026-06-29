using Content.Shared._RW.Skin;
using Robust.Client.GameObjects;
using Robust.Shared.Utility;

namespace Content.Client._RW.Skin;

/// <summary>
///     Client-side override of the skin system to handle SpriteComponent rendering.
/// </summary>
public sealed class ItemSkinSystem : SharedItemSkinSystem
{
    protected override void UpdateSprite(EntityUid uid, AppliedItemSkinComponent component)
    {
        if (TryComp<SpriteComponent>(uid, out var sprite))
        {
            if (!string.IsNullOrEmpty(component.SpriteRsi))
            {
                var state = component.SpriteState;
                if (string.IsNullOrEmpty(state) && sprite.TryGetLayer(0, out var layer))
                {
                    state = layer.State.Name;
                }

                sprite.LayerSetSprite(0, new SpriteSpecifier.Rsi(new ResPath(component.SpriteRsi), state ?? "icon"));
            }
        }
    }
}
