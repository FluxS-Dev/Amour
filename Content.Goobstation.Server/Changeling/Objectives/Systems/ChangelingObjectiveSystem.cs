// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2024 whateverusername0 <whateveremail>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 GoobBot <uristmchands@proton.me>
// SPDX-FileCopyrightText: 2025 Marcus F <199992874+thebiggestbruh@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Misandry <mary@thughunt.ing>
// SPDX-FileCopyrightText: 2025 gluesniffler <159397573+gluesniffler@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 gus <august.eymann@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Goobstation.Server.Changeling.Objectives.Components;
using Content.Goobstation.Shared.Changeling.Components;
using Content.Server.Objectives.Systems;
using Content.Goobstation.Server._RW.Objectives.Components;
using Content.Shared.Objectives.Components;

namespace Content.Goobstation.Server.Changeling.Objectives.Systems;

public sealed partial class ChangelingObjectiveSystem : EntitySystem
{
    [Dependency] private readonly NumberObjectiveSystem _number = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AbsorbConditionComponent, ObjectiveGetProgressEvent>(OnAbsorbGetProgress);
        SubscribeLocalEvent<StealDNAConditionComponent, ObjectiveGetProgressEvent>(OnStealDNAGetProgress);
        SubscribeLocalEvent<AbsorbChangelingConditionComponent, ObjectiveGetProgressEvent>(OnAbsorbChangelingGetProgress);
        SubscribeLocalEvent<ChangelingEvolutionaryApexConditionComponent, ObjectiveGetProgressEvent>(OnAbsorbMoreGetProgress); // RW
    }

    private void OnAbsorbGetProgress(EntityUid uid, AbsorbConditionComponent comp, ref ObjectiveGetProgressEvent args)
    {
        var target = _number.GetTarget(uid);
        if (target != 0)
            args.Progress = MathF.Min(comp.Absorbed / target, 1f);
        else args.Progress = 1f;
    }
    private void OnStealDNAGetProgress(EntityUid uid, StealDNAConditionComponent comp, ref ObjectiveGetProgressEvent args)
    {
        var target = _number.GetTarget(uid);
        if (target != 0)
            args.Progress = MathF.Min(comp.DNAStolen / target, 1f);
        else args.Progress = 1f;
    }
    private void OnAbsorbChangelingGetProgress(EntityUid uid, AbsorbChangelingConditionComponent comp, ref ObjectiveGetProgressEvent args)
    {
        var target = _number.GetTarget(uid);
        if (target != 0)
            args.Progress = MathF.Min(comp.LingAbsorbed / target, 1f);
        else
            args.Progress = 1f;
    }

    // RW start
    private void OnAbsorbMoreGetProgress(EntityUid uid, ChangelingEvolutionaryApexConditionComponent comp, ref ObjectiveGetProgressEvent args)
    {
        var selfEntity = args.Mind.OwnedEntity;
        if (selfEntity == null)
        {
            args.Progress = 0f;
            return;
        }

        if (!TryComp<ChangelingIdentityComponent>(selfEntity, out var selfIdentity))
        {
            args.Progress = 0f;
            return;
        }

        var selfAbsorbed = selfIdentity.TotalAbsorbedEntities;
        var maxOtherAbsorbed = -1;
        var hasOtherLings = false;

        var query = EntityQueryEnumerator<ChangelingIdentityComponent>();
        while (query.MoveNext(out var lingUid, out var lingComp))
        {
            if (lingUid == selfEntity)
                continue;

            hasOtherLings = true;
            if (lingComp.TotalAbsorbedEntities > maxOtherAbsorbed)
            {
                maxOtherAbsorbed = lingComp.TotalAbsorbedEntities;
            }
        }

        if (hasOtherLings)
        {
            args.Progress = selfAbsorbed > maxOtherAbsorbed ? 1f : 0f;
        }
        else
        {
            // If there are no other lings, this objective is trivially met.
            args.Progress = 1f;
        }
    }
    // RW end
}
