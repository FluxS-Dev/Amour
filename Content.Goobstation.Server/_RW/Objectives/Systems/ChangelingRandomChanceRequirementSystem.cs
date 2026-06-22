using Content.Goobstation.Server._RW.Objectives.Components;
using Content.Shared.Objectives.Components;
using Robust.Shared.Random;

namespace Content.Goobstation.Server._RW.Objectives.Systems;

public sealed class ChangelingRandomChanceRequirementSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ChangelingRandomChanceRequirementComponent, RequirementCheckEvent>(OnCheck);
    }

    private void OnCheck(EntityUid uid, ChangelingRandomChanceRequirementComponent comp, ref RequirementCheckEvent args)
    {
        if (args.Cancelled)
            return;

        if (!_random.Prob(comp.Chance))
            args.Cancelled = true;
    }
}
