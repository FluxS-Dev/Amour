using Content.Shared.Body.Organ;
using Content.Shared.Body.Systems;
using Content.Shared.Examine;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared._Shitmed.Body.Organ;
using Content.Shared._Shitmed.Medical.Surgery;
using Content.Shared._Shitmed.Medical.Surgery.Steps;
using Robust.Shared.Containers;

namespace Content.Shared._RW.LifeTrigger;

public sealed class SharedLifeTriggerSystem : EntitySystem
{
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly SharedBodySystem _body = default!;
    [Dependency] private readonly SharedContainerSystem _container = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<LifeTriggerComponent, UseInHandEvent>(OnUseInHand);
        SubscribeLocalEvent<LifeTriggerComponent, GetVerbsEvent<AlternativeVerb>>(OnGetVerbs);
        SubscribeLocalEvent<LifeTriggerComponent, ExaminedEvent>(OnExamine);

        SubscribeLocalEvent<SurgeryStepAttachLifeTriggerComponent, SurgeryStepEvent>(OnAttachStep);
        SubscribeLocalEvent<SurgeryStepAttachLifeTriggerComponent, SurgeryStepCompleteCheckEvent>(OnAttachCheck);
    }

    private void OnUseInHand(EntityUid uid, LifeTriggerComponent component, UseInHandEvent args)
    {
        if (args.Handled)
            return;

        component.TriggerState = component.TriggerState switch
        {
            MobState.Dead => MobState.Critical,
            MobState.Critical => MobState.HardCritical,
            MobState.HardCritical => MobState.Dead,
            _ => MobState.Dead
        };
        Dirty(uid, component);

        var stateStr = GetStateName(component.TriggerState);
        _popup.PopupClient(Loc.GetString("life-trigger-switched", ("state", stateStr)), uid, args.User);
        args.Handled = true;
    }

    private void OnGetVerbs(EntityUid uid, LifeTriggerComponent component, GetVerbsEvent<AlternativeVerb> args)
    {
        if (!args.CanAccess || !args.CanInteract)
            return;

        foreach (var state in new[] { MobState.Dead, MobState.Critical, MobState.HardCritical })
        {
            var stateName = GetStateName(state);
            var verbState = state;
            var verb = new AlternativeVerb
            {
                Text = Loc.GetString("life-trigger-verb-set-state", ("state", stateName)),
                Act = () =>
                {
                    component.TriggerState = verbState;
                    Dirty(uid, component);
                    _popup.PopupClient(Loc.GetString("life-trigger-switched", ("state", stateName)), uid, args.User);
                },
                Disabled = component.TriggerState == state,
                Priority = (int) state
            };
            args.Verbs.Add(verb);
        }
    }

    private void OnExamine(EntityUid uid, LifeTriggerComponent component, ExaminedEvent args)
    {
        args.PushMarkup(Loc.GetString("life-trigger-examine", ("state", GetStateName(component.TriggerState))));
    }

    private string GetStateName(MobState state)
    {
        return state switch
        {
            MobState.Dead => Loc.GetString("life-trigger-state-dead"),
            MobState.Critical => Loc.GetString("life-trigger-state-critical"),
            MobState.HardCritical => Loc.GetString("life-trigger-state-severe-critical"),
            _ => Loc.GetString("life-trigger-state-dead")
        };
    }

    private void OnAttachStep(Entity<SurgeryStepAttachLifeTriggerComponent> ent, ref SurgeryStepEvent args)
    {
        var organs = _body.GetPartOrgans(args.Part);

        EntityUid? heartUid = null;
        foreach (var organ in organs)
        {
            if (HasComp<HeartComponent>(organ.Id))
            {
                heartUid = organ.Id;
                break;
            }
        }

        if (heartUid == null)
            return;

        if (!TryComp<LifeTriggerComponent>(args.Tool, out _))
            return;

        var container = _container.EnsureContainer<ContainerSlot>(heartUid.Value, "life-trigger-slot");
        if (container.ContainedEntity != null)
            return;

        if (_container.Insert(args.Tool, container))
        {
            _appearance.SetData(heartUid.Value, CardiacLifeTriggerVisuals.HasTrigger, true);
            args.Complete = true;
        }
    }

    private void OnAttachCheck(Entity<SurgeryStepAttachLifeTriggerComponent> ent, ref SurgeryStepCompleteCheckEvent args)
    {
        var organs = _body.GetPartOrgans(args.Part);

        EntityUid? heartUid = null;
        foreach (var organ in organs)
        {
            if (HasComp<HeartComponent>(organ.Id))
            {
                heartUid = organ.Id;
                break;
            }
        }

        if (heartUid == null)
        {
            args.Cancelled = true;
            return;
        }

        var container = _container.EnsureContainer<ContainerSlot>(heartUid.Value, "life-trigger-slot");
        if (container.ContainedEntity == null)
        {
            args.Cancelled = true;
        }
    }
}
