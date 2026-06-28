using Content.Shared._White.RadialSelector;
using Content.Shared.UserInterface;
using Robust.Shared.GameObjects;

namespace Content.Shared.ShortConstruction;

public sealed class ShortConstructionSystem : EntitySystem
{
    [Dependency] private readonly SharedUserInterfaceSystem _ui = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ShortConstructionComponent, BeforeActivatableUIOpenEvent>(BeforeUiOpen);
        SubscribeLocalEvent<ShortConstructionComponent, BoundUIOpenedEvent>(OnUiOpened);
    }

    private void BeforeUiOpen(Entity<ShortConstructionComponent> ent, ref BeforeActivatableUIOpenEvent args)
    {
        SetUiState(ent);
    }

    private void OnUiOpened(Entity<ShortConstructionComponent> ent, ref BoundUIOpenedEvent args)
    {
        SetUiState(ent);
    }

    private void SetUiState(Entity<ShortConstructionComponent> ent)
    {
        _ui.SetUiState(ent.Owner, RadialSelectorUiKey.Key, new RadialSelectorState(ent.Comp.Entries));
    }
}
