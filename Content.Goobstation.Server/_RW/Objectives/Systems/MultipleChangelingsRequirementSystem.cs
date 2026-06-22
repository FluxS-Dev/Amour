using Content.Goobstation.Server._RW.Objectives.Components;
using Content.Goobstation.Shared.Changeling.Components;
using Content.Shared.Objectives.Components;

namespace Content.Goobstation.Server._RW.Objectives.Systems;

public sealed class MultipleChangelingsRequirementSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MultipleChangelingsRequirementComponent, RequirementCheckEvent>(OnCheck);
    }

    private void OnCheck(EntityUid uid, MultipleChangelingsRequirementComponent comp, ref RequirementCheckEvent args)
    {
        if (args.Cancelled)
            return;

        var selfEntity = args.Mind.OwnedEntity;
        var count = 0;

        var query = EntityQueryEnumerator<ChangelingIdentityComponent>();
        while (query.MoveNext(out var lingUid, out _))
        {
            if (lingUid != selfEntity)
                count++;
        }

        if (count < comp.Changelings)
            args.Cancelled = true;
    }
}
