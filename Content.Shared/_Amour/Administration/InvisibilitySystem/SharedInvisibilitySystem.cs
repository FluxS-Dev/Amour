using Content.Shared.Actions;
using Content.Shared.Examine;
using Robust.Shared.Serialization;

namespace Content.Shared._Amour.Administration;

public abstract class SharedInvisibilitySystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actions = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<InvisibilityComponent, ExaminedEvent>(OnExamined);
    }

    private void OnExamined(EntityUid uid, InvisibilityComponent component, ExaminedEvent args)
    {
        if (component.Invisible)
            args.PushMarkup("[color=lightsteelblue]Оно доступно лишь взору богов.[/color]");
    }
}

[Serializable, NetSerializable]
public sealed class InvisibilityToggleEvent(NetEntity uid, bool invisible) : EntityEventArgs
{
    public NetEntity Uid { get; } = uid;
    public bool Invisible { get; } = invisible;
}
