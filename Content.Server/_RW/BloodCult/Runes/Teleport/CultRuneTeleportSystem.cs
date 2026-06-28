using Content.Server.Popups;
using Content.Shared._White.ListViewSelector;
using Content.Shared.UserInterface;
using Content.Shared._RW.BloodCult.UI;
using Robust.Server.Audio;
using Robust.Server.GameObjects;

namespace Content.Server._RW.BloodCult.Runes.Teleport;

public sealed class CultRuneTeleportSystem : EntitySystem
{
    [Dependency] private readonly AudioSystem _audio = default!;
    [Dependency] private readonly CultRuneBaseSystem _cultRune = default!;
    [Dependency] private readonly TransformSystem _transform = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly UserInterfaceSystem _ui = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CultRuneTeleportComponent, AfterRunePlaced>(OnAfterRunePlaced);
        SubscribeLocalEvent<CultRuneTeleportComponent, NameSelectedMessage>(OnNameSelected);
        SubscribeLocalEvent<CultRuneTeleportComponent, BoundUIClosedEvent>(OnNameSelectorClosed);
        SubscribeLocalEvent<CultRuneTeleportComponent, TryInvokeCultRuneEvent>(OnTeleportRuneInvoked);
        SubscribeLocalEvent<CultRuneTeleportComponent, ListViewItemSelectedMessage>(OnTeleportRuneSelected);
    }

    private void OnAfterRunePlaced(Entity<CultRuneTeleportComponent> rune, ref AfterRunePlaced args)
    {
        _ui.OpenUi(rune.Owner, NameSelectorUiKey.Key, args.User);
    }

    private void OnNameSelected(Entity<CultRuneTeleportComponent> rune, ref NameSelectedMessage args)
    {
        rune.Comp.Name = ResolveTeleportRuneName(args.Name);
    }

    private void OnNameSelectorClosed(Entity<CultRuneTeleportComponent> rune, ref BoundUIClosedEvent args)
    {
        if (args.UiKey is not NameSelectorUiKey || !string.IsNullOrWhiteSpace(rune.Comp.Name))
            return;

        rune.Comp.Name = ResolveTeleportRuneName(null);
    }

    private void OnTeleportRuneInvoked(Entity<CultRuneTeleportComponent> rune, ref TryInvokeCultRuneEvent args)
    {
        var runeUid = rune.Owner;
        if (_ui.IsUiOpen(runeUid, ListViewSelectorUiKey.Key))
        {
            args.Cancel();
            return;
        }

        if (!TryGetTeleportRunes(args.User, out var runes, runeUid))
        {
            args.Cancel();
            return;
        }

        _ui.SetUiState(runeUid, ListViewSelectorUiKey.Key, new ListViewSelectorState(runes));
        _ui.TryToggleUi(runeUid, ListViewSelectorUiKey.Key, args.User);
    }

    private void OnTeleportRuneSelected(Entity<CultRuneTeleportComponent> origin, ref ListViewItemSelectedMessage args)
    {
        if (!EntityUid.TryParse(args.SelectedItem.Id, out var destination))
            return;

        var teleportTargets = _cultRune.GetTargetsNearRune(origin, origin.Comp.TeleportGatherRange);
        var destinationTransform = Transform(destination);

        foreach (var target in teleportTargets)
        {
            _cultRune.StopPulling(target);
            _transform.SetCoordinates(target, destinationTransform.Coordinates);
        }

        _audio.PlayPvs(origin.Comp.TeleportOutSound, origin);
        _audio.PlayPvs(origin.Comp.TeleportInSound, destination);
    }

    public bool TryGetTeleportRunes(EntityUid user, out List<ListViewSelectorEntry> runes, EntityUid? runeUid = null)
    {
        var runeQuery = EntityQueryEnumerator<CultRuneTeleportComponent>();
        runes = new List<ListViewSelectorEntry>();
        while (runeQuery.MoveNext(out var targetRune, out var teleportRune))
        {
            if (targetRune == runeUid)
                continue;

            var entry = new ListViewSelectorEntry(targetRune.ToString(), ResolveTeleportRuneName(teleportRune.Name));
            runes.Add(entry);
        }

        if (runes.Count != 0)
            return true;

        _popup.PopupEntity(Loc.GetString("cult-teleport-not-found"), user, user);
        return false;
    }

    private string ResolveTeleportRuneName(string? name) =>
        string.IsNullOrWhiteSpace(name) ? Loc.GetString("cult-teleport-rune-unnamed") : name.Trim();
}
