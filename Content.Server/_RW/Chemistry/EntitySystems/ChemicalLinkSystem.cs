using Content.Server.Chemistry.Components;
using Content.Server.Tools;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Tools.Systems;
using Robust.Server.Audio;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;

namespace Content.Server.Chemistry.EntitySystems
{
    public sealed class ChemicalLinkSystem : EntitySystem
    {
        [Dependency] private readonly SharedTransformSystem _transformSystem = default!;
        [Dependency] private readonly SharedPopupSystem _popupSystem = default!;
        [Dependency] private readonly ToolSystem _toolSystem = default!;
        [Dependency] private readonly AudioSystem _audioSystem = default!;
        [Dependency] private readonly IComponentFactory _componentFactory = default!;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<ReagentDispenserComponent, InteractUsingEvent>(OnInteractUsingDispenser);
            SubscribeLocalEvent<ChemMasterComponent, InteractUsingEvent>(OnInteractUsingMaster);
            SubscribeLocalEvent<ChemicalLinkComponent, ComponentShutdown>(OnShutdown);
        }

        private void OnInteractUsingDispenser(EntityUid uid, ReagentDispenserComponent component, InteractUsingEvent args)
        {
            if (args.Handled)
                return;

            if (!_toolSystem.HasQuality(args.Used, SharedToolSystem.PulseQuality))
                return;

            args.Handled = true;
            HandleLinking(args.Used, uid, args.User, isDispenser: true);
        }

        private void OnInteractUsingMaster(EntityUid uid, ChemMasterComponent component, InteractUsingEvent args)
        {
            if (args.Handled)
                return;

            if (!_toolSystem.HasQuality(args.Used, SharedToolSystem.PulseQuality))
                return;

            args.Handled = true;
            HandleLinking(args.Used, uid, args.User, isDispenser: false);
        }

        public void HandleLinking(EntityUid tool, EntityUid machine, EntityUid user, bool isDispenser)
        {
            var linker = EnsureComp<ChemicalLinkerComponent>(tool);

            if (linker.SavedDevice == null)
            {
                linker.SavedDevice = machine;
                var machineName = Name(machine);
                _popupSystem.PopupEntity(Loc.GetString("chemical-linker-saved-device", ("device", machineName)), machine, user);
                _audioSystem.PlayPvs("/Audio/Machines/machine_switch.ogg", machine, AudioParams.Default.WithVolume(-2f));
                return;
            }

            var saved = linker.SavedDevice.Value;
            if (!Exists(saved))
            {
                linker.SavedDevice = machine;
                var machineName = Name(machine);
                _popupSystem.PopupEntity(Loc.GetString("chemical-linker-saved-device", ("device", machineName)), machine, user);
                _audioSystem.PlayPvs("/Audio/Machines/machine_switch.ogg", machine, AudioParams.Default.WithVolume(-2f));
                return;
            }

            if (saved == machine)
            {
                linker.SavedDevice = null;
                if (TryComp<ChemicalLinkComponent>(machine, out var currentLink))
                {
                    var linked = currentLink.LinkedDevice;
                    RemCompDeferred<ChemicalLinkComponent>(machine);
                    if (linked.HasValue && Exists(linked.Value))
                    {
                        RemCompDeferred<ChemicalLinkComponent>(linked.Value);
                    }
                    _popupSystem.PopupEntity(Loc.GetString("chemical-linker-unlinked"), machine, user);
                }
                else
                {
                    _popupSystem.PopupEntity(Loc.GetString("chemical-linker-unlinked"), machine, user);
                }
                _audioSystem.PlayPvs("/Audio/Machines/machine_switch.ogg", machine, AudioParams.Default.WithVolume(-2f));
                return;
            }

            var savedIsDispenser = IsDispenser(saved);
            var savedIsMaster = HasComp<ChemMasterComponent>(saved);

            if ((isDispenser && savedIsDispenser) || (!isDispenser && savedIsMaster))
            {
                linker.SavedDevice = machine;
                var machineName = Name(machine);
                _popupSystem.PopupEntity(Loc.GetString("chemical-linker-saved-device", ("device", machineName)), machine, user);
                _audioSystem.PlayPvs("/Audio/Machines/machine_switch.ogg", machine, AudioParams.Default.WithVolume(-2f));
                return;
            }

            var dispenser = isDispenser ? machine : saved;
            var master = isDispenser ? saved : machine;

            if (!_transformSystem.InRange(dispenser, master, 1.5f))
            {
                _popupSystem.PopupEntity(Loc.GetString("chemical-linker-not-adjacent"), machine, user);
                return;
            }

            var dispenserLink = EnsureComp<ChemicalLinkComponent>(dispenser);
            dispenserLink.LinkedDevice = master;

            var masterLink = EnsureComp<ChemicalLinkComponent>(master);
            masterLink.LinkedDevice = dispenser;

            _popupSystem.PopupEntity(Loc.GetString("chemical-linker-linked", ("dispenser", Name(dispenser)), ("master", Name(master))), machine, user);
            _audioSystem.PlayPvs("/Audio/Machines/high_tech_confirm.ogg", machine, AudioParams.Default.WithVolume(-1f));

            linker.SavedDevice = null;
        }

        private bool IsDispenser(EntityUid uid)
        {
            if (HasComp<ReagentDispenserComponent>(uid))
                return true;

            if (_componentFactory.TryGetRegistration("EnergyReagentDispenser", out var registration) && HasComp(uid, registration.Type))
                return true;

            return false;
        }

        private void OnShutdown(EntityUid uid, ChemicalLinkComponent component, ComponentShutdown args)
        {
            if (component.LinkedDevice is { } linked && Exists(linked) && HasComp<ChemicalLinkComponent>(linked))
            {
                RemCompDeferred<ChemicalLinkComponent>(linked);
            }
        }
    }
}
