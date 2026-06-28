using Content.Shared._RW.LifeTrigger;
using Content.Shared._Shitmed.Body.Organ;
using Robust.Client.GameObjects;
using Robust.Shared.Utility;

namespace Content.Client._RW.LifeTrigger;

public sealed class LifeTriggerVisualsSystem : VisualizerSystem<HeartComponent>
{
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly SpriteSystem _sprite = default!;

    protected override void OnAppearanceChange(EntityUid uid, HeartComponent component, ref AppearanceChangeEvent args)
    {
        if (args.Sprite == null)
            return;

        if (!_appearance.TryGetData<bool>(uid, CardiacLifeTriggerVisuals.HasTrigger, out var hasTrigger, args.Component))
            hasTrigger = false;

        var mapKey = "trigger";
        var rsi = new SpriteSpecifier.Rsi(new ResPath("_RW/Objects/Specific/Medical/life_trigger.rsi"), "heart-clothed");

        if (hasTrigger)
        {
            if (!_sprite.LayerMapTryGet((uid, args.Sprite), mapKey, out var idx, false))
            {
                var newIdx = _sprite.AddLayer((uid, args.Sprite), rsi);
                _sprite.LayerMapSet((uid, args.Sprite), mapKey, newIdx);
                idx = newIdx;
            }
            _sprite.LayerSetVisible((uid, args.Sprite), idx, true);
        }
        else
        {
            if (_sprite.LayerMapTryGet((uid, args.Sprite), mapKey, out var idx, false))
            {
                _sprite.LayerSetVisible((uid, args.Sprite), idx, false);
            }
        }
    }
}
