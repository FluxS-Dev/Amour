// SPDX-FileCopyrightText: 2023 Chief-Engineer <119664036+Chief-Engineer@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 ShadowCommander <10494922+ShadowCommander@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using Content.Server.Anomaly.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.Ghost;
using Content.Shared.Mind.Components;
using Content.Shared.Teleportation.Systems;
using Robust.Shared.Map;

namespace Content.Server.Teleportation;

public sealed class PortalSystem : SharedPortalSystem
{
    [Dependency] private readonly ISharedAdminLogManager _adminLogger = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;

    // TODO Move to shared
    protected override void LogTeleport(EntityUid portal, EntityUid subject, EntityCoordinates source,
        EntityCoordinates target)
    {
        if (HasComp<MindContainerComponent>(subject) && !HasComp<GhostComponent>(subject))
            _adminLogger.Add(LogType.Teleport, LogImpact.Low, $"{ToPrettyString(subject):player} teleported via {ToPrettyString(portal)} from {source} to {target}");

        // RW start
        if (TryComp<BluespaceAnomalyComponent>(portal, out var bluespace))
        {
            _damageable.TryChangeDamage(subject, bluespace.TeleportDamage, true);
        }
        // RW end
    }
}