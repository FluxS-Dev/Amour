using Robust.Shared.Audio.Systems;
using Content.Shared.Interaction;
using Content.Shared.Item;
using Content.Shared.Clothing.Components;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Examine;
using Content.Shared.Popups;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._RW.Skin;

/// <summary>
///     Handles the case interaction and updating visuals on items with applied skins.
/// </summary>
public abstract class SharedItemSkinSystem : EntitySystem
{
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly SharedItemSystem _item = default!;
    [Dependency] private readonly ClothingSystem _clothing = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ItemComponent, InteractUsingEvent>(OnInteractUsing);
        SubscribeLocalEvent<AppliedItemSkinComponent, AfterAutoHandleStateEvent>(OnSkinStateHandled);
        SubscribeLocalEvent<AppliedItemSkinComponent, ComponentStartup>(OnSkinStartup);
        SubscribeLocalEvent<ItemSkinCaseComponent, ExaminedEvent>(OnCaseExamined);
    }

    private void OnSkinStateHandled(EntityUid uid, AppliedItemSkinComponent component, ref AfterAutoHandleStateEvent args)
    {
        UpdateVisuals(uid, component);
    }

    private void OnSkinStartup(EntityUid uid, AppliedItemSkinComponent component, ComponentStartup args)
    {
        UpdateVisuals(uid, component);
    }

    private void OnInteractUsing(Entity<ItemComponent> ent, ref InteractUsingEvent args)
    {
        if (args.Handled)
            return;

        if (!HasComp<ItemSkinCaseComponent>(args.Used))
            return;

        if (TryApplySkinCase(args.User, args.Used, ent))
        {
            args.Handled = true;
        }
    }

    public bool TryApplySkinCase(EntityUid user, EntityUid caseUid, EntityUid target, ItemSkinCaseComponent? caseComp = null)
    {
        if (!Resolve(caseUid, ref caseComp, false))
            return false;

        if (!CanApplySkinCase(user, (caseUid, caseComp), target, out var error))
        {
            if (_net.IsServer && error != null)
            {
                _popup.PopupEntity(Loc.GetString(error), target, user, PopupType.MediumCaution);
            }
            return false;
        }

        if (_net.IsServer)
        {
            var applied = EnsureComp<AppliedItemSkinComponent>(target);
            applied.SpriteRsi = caseComp.SpriteRsi;
            applied.SpriteState = caseComp.SpriteState;
            applied.InhandRsi = caseComp.InhandRsi;
            applied.ClothingRsi = caseComp.ClothingRsi;

            Dirty(target, applied);
            UpdateVisuals(target, applied);

            _popup.PopupEntity(Loc.GetString("item-skin-case-applied", ("item", target)), target, user);
            _audio.PlayPvs(caseComp.ApplySound, target);

            caseComp.Uses--;
            if (caseComp.Uses <= 0)
            {
                QueueDel(caseUid);
            }
            else
            {
                Dirty(caseUid, caseComp);
            }
        }

        return true;
    }

    public bool CanApplySkinCase(EntityUid user, Entity<ItemSkinCaseComponent> caseEnt, EntityUid target, out string? error)
    {
        error = null;

        if (caseEnt.Comp.Uses <= 0)
        {
            error = "item-skin-case-no-uses";
            return false;
        }

        var targetProto = MetaData(target).EntityPrototype;
        if (targetProto == null || targetProto.ID != caseEnt.Comp.TargetPrototype)
        {
            error = "item-skin-case-wrong-item";
            return false;
        }

        return true;
    }

    private void OnCaseExamined(Entity<ItemSkinCaseComponent> ent, ref ExaminedEvent args)
    {
        args.PushMarkup(Loc.GetString("item-skin-case-uses-remaining", ("uses", ent.Comp.Uses)));
    }

    public void UpdateVisuals(EntityUid uid, AppliedItemSkinComponent component)
    {
        // 1. Client-side sprite update (handled by client system override)
        UpdateSprite(uid, component);

        // 2. In-hand visuals
        if (TryComp<ItemComponent>(uid, out var item))
        {
            if (!string.IsNullOrEmpty(component.InhandRsi))
            {
                item.RsiPath = component.InhandRsi;
                item.InhandVisuals.Clear();
            }
        }

        // 3. Clothing visuals
        if (TryComp<ClothingComponent>(uid, out var clothing))
        {
            if (!string.IsNullOrEmpty(component.ClothingRsi))
            {
                clothing.RsiPath = component.ClothingRsi;
                clothing.ClothingVisuals.Clear();
            }
        }

        // 4. Update the hands/inventory system
        _item.VisualsChanged(uid);
    }

    protected virtual void UpdateSprite(EntityUid uid, AppliedItemSkinComponent component)
    {
    }
}
