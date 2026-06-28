using Content.Shared.Body.Organ;
using Content.Shared.Body.Systems;
using Content.Shared.DeviceLinking;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Mobs;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared._RW.LifeTrigger;
using Content.Shared._Shitmed.Body.Organ;
using Content.Shared._Shitmed.Medical.Surgery;
using Content.Shared._Shitmed.Medical.Surgery.Steps;
using Content.Shared._Shitmed.Medical.Surgery.Steps.Parts;
using Robust.Shared.Containers;
using System.Collections.Generic;
using Content.Server.DeviceLinking.Systems;
using Content.Shared.Body.Components;
using Robust.Shared.Prototypes;

namespace Content.Server._RW.LifeTrigger;

public sealed class LifeTriggerSystem : EntitySystem
{
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly SharedBodySystem _body = default!;
    [Dependency] private readonly SharedContainerSystem _container = default!;
    [Dependency] private readonly DeviceLinkSystem _deviceLink = default!;
    [Dependency] private readonly SharedHandsSystem _hands = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MobStateChangedEvent>(OnMobStateChanged);

        SubscribeLocalEvent<HeartComponent, GetVerbsEvent<AlternativeVerb>>(OnHeartGetVerbs);
        SubscribeLocalEvent<HeartComponent, InteractUsingEvent>(OnHeartInteractUsing);
    }

    private void OnMobStateChanged(MobStateChangedEvent args)
    {
        var target = args.Target;
        var triggerEntOpt = GetLifeTriggerEntity(target);
        if (triggerEntOpt is not { } triggerEnt)
            return;

        var trigger = triggerEnt.Comp;
        var triggerUid = triggerEnt.Owner;

        if (args.NewMobState == trigger.TriggerState)
        {
            if (trigger.LastTriggeredState == args.NewMobState)
                return;

            trigger.LastTriggeredState = args.NewMobState;
            Dirty(triggerUid, trigger);

            InvokePortWithRangeCheck(triggerUid, trigger.Port);
        }
        else
        {
            if (trigger.LastTriggeredState == trigger.TriggerState)
            {
                trigger.LastTriggeredState = null;
                Dirty(triggerUid, trigger);
            }
        }
    }

    private void OnHeartGetVerbs(EntityUid uid, HeartComponent component, GetVerbsEvent<AlternativeVerb> args)
    {
        if (!args.CanAccess || !args.CanInteract)
            return;

        if (!_container.TryGetContainer(uid, "life-trigger-slot", out var container) ||
            container is not ContainerSlot slot ||
            slot.ContainedEntity is not { } triggerUid)
            return;

        var isLoose = !TryComp<OrganComponent>(uid, out var organComp) || organComp.Body == null;
        var isOpen = false;
        if (organComp != null && organComp.Body is { } bodyUid)
        {
            if (_container.TryGetContainingContainer(uid, out var organContainer) &&
                HasComp<BonesOpenComponent>(organContainer.Owner))
            {
                isOpen = true;
            }
        }

        if (!isLoose && !isOpen)
            return;

        var verb = new AlternativeVerb
        {
            Text = Loc.GetString("life-trigger-verb-eject"),
            Act = () =>
            {
                if (_container.Remove(triggerUid, container))
                {
                    _appearance.SetData(uid, CardiacLifeTriggerVisuals.HasTrigger, false);
                    _hands.PickupOrDrop(args.User, triggerUid);
                    _popup.PopupEntity(Loc.GetString("life-trigger-ejected"), uid, args.User);
                }
            }
        };
        args.Verbs.Add(verb);
    }

    private void OnHeartInteractUsing(EntityUid uid, HeartComponent component, InteractUsingEvent args)
    {
        if (args.Handled)
            return;

        if (!TryComp<LifeTriggerComponent>(args.Used, out _))
            return;

        if (TryComp<OrganComponent>(uid, out var organ) && organ.Body != null)
            return; // Use surgery if inside a body!

        var container = _container.EnsureContainer<ContainerSlot>(uid, "life-trigger-slot");
        if (container.ContainedEntity != null)
            return;

        if (_container.Insert(args.Used, container))
        {
            _appearance.SetData(uid, CardiacLifeTriggerVisuals.HasTrigger, true);
            _popup.PopupEntity(Loc.GetString("life-trigger-attached"), uid, args.User);
            args.Handled = true;
        }
    }

    private Entity<LifeTriggerComponent>? GetLifeTriggerEntity(EntityUid body)
    {
        if (!HasComp<BodyComponent>(body))
            return null;

        foreach (var organ in _body.GetBodyOrgans(body))
        {
            if (!HasComp<HeartComponent>(organ.Id))
                continue;

            if (_container.TryGetContainer(organ.Id, "life-trigger-slot", out var container) &&
                container is ContainerSlot slot &&
                slot.ContainedEntity is { } triggerUid &&
                TryComp<LifeTriggerComponent>(triggerUid, out var trigger))
            {
                return (triggerUid, trigger);
            }
        }

        return null;
    }

    private void InvokePortWithRangeCheck(EntityUid uid, ProtoId<SourcePortPrototype> port)
    {
        if (!TryComp<DeviceLinkSourceComponent>(uid, out var sourceComponent))
            return;

        var originalOutputs = new Dictionary<ProtoId<SourcePortPrototype>, List<EntityUid>>();
        var triggerPos = _transform.GetMapCoordinates(uid);

        foreach (var (portId, sinks) in sourceComponent.Outputs)
        {
            originalOutputs[portId] = new List<EntityUid>(sinks);
            var inRangeSinks = new List<EntityUid>();
            foreach (var sink in sinks)
            {
                var sinkPos = _transform.GetMapCoordinates(sink);
                if (triggerPos.MapId == sinkPos.MapId && triggerPos.InRange(sinkPos, sourceComponent.Range))
                {
                    inRangeSinks.Add(sink);
                }
            }

            var sinksSet = sourceComponent.Outputs[portId];
            sinksSet.Clear();
            foreach (var sink in inRangeSinks)
            {
                sinksSet.Add(sink);
            }
        }

        // Call InvokePort
        _deviceLink.InvokePort(uid, port, null, sourceComponent);

        // Restore original outputs
#pragma warning disable RA0002
        foreach (var (portId, sinks) in originalOutputs)
        {
            if (sourceComponent.Outputs.TryGetValue(portId, out var sinksSet))
            {
                sinksSet.Clear();
                foreach (var sink in sinks)
                {
                    sinksSet.Add(sink);
                }
            }
        }
#pragma warning restore RA0002
    }
}
