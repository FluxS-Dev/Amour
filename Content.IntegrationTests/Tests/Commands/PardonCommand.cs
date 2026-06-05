// SPDX-FileCopyrightText: 2021 Acruid <shatter66@gmail.com>
// SPDX-FileCopyrightText: 2021 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 metalgearsloth <comedian_vs_clown@hotmail.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 TemporalOroboros <TemporalOroboros@gmail.com>
// SPDX-FileCopyrightText: 2023 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2024 Tayrtahn <tayrtahn@gmail.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using Content.Server.Database;
using Robust.Server.Console;
using Robust.Server.Player;
using Robust.Shared.Network;

namespace Content.IntegrationTests.Tests.Commands
{
    [TestFixture]
    [TestOf(typeof(PardonCommand))]
    public sealed class PardonCommand
    {
        private static readonly TimeSpan MarginOfError = TimeSpan.FromMinutes(1);

        [Test]
        public async Task PardonTest()
        {
            await using var pair = await PoolManager.GetServerClient(new PoolSettings { Connected = true });
            var server = pair.Server;
            var client = pair.Client;

            var sPlayerManager = server.ResolveDependency<IPlayerManager>();
            var sConsole = server.ResolveDependency<IServerConsoleHost>();
            var sDatabase = server.ResolveDependency<IServerDbManager>();
            var netMan = client.ResolveDependency<IClientNetManager>();
            var clientSession = sPlayerManager.Sessions.Single();
            var clientId = clientSession.UserId;
            const int missingBanId = int.MaxValue; // Amour edit: pooled tests may leave pardoned bans in the database.

            Assert.That(netMan.IsConnected);

            Assert.That(sPlayerManager.Sessions, Has.Length.EqualTo(1));
            // No bans on record
            Assert.Multiple(async () =>
            {
                Assert.That(await sDatabase.GetServerBanAsync(null, clientId, null, null), Is.Null);
                Assert.That(await sDatabase.GetServerBanAsync(missingBanId), Is.Null); // Amour edit
                Assert.That(await sDatabase.GetServerBansAsync(null, clientId, null, null), Is.Empty);
            });

            // Try to pardon a ban that does not exist
            await server.WaitPost(() => sConsole.ExecuteCommand($"pardon {missingBanId}")); // Amour edit

            // Still no bans on record
            Assert.Multiple(async () =>
            {
                Assert.That(await sDatabase.GetServerBanAsync(null, clientId, null, null), Is.Null);
                Assert.That(await sDatabase.GetServerBanAsync(missingBanId), Is.Null); // Amour edit
                Assert.That(await sDatabase.GetServerBansAsync(null, clientId, null, null), Is.Empty);
            });

            var banReason = "test";

            Assert.That(sPlayerManager.Sessions, Has.Length.EqualTo(1));
            // Amour edit start: use a permanent ban and do not assume it is the first row in a pooled database.
            // Ban the client permanently
            await server.WaitPost(() => sConsole.ExecuteCommand($"ban {clientSession.Name} {banReason} 0"));
            var ban = await sDatabase.GetServerBanAsync(null, clientId, null, null);
            Assert.That(ban?.Id, Is.Not.Null);
            var banId = ban!.Id!.Value;
            // Amour edit end

            // Should have one ban on record now
            Assert.Multiple(async () =>
            {
                Assert.That(await sDatabase.GetServerBanAsync(null, clientId, null, null), Is.Not.Null);
                Assert.That(await sDatabase.GetServerBanAsync(banId), Is.Not.Null); // Amour edit
                Assert.That(await sDatabase.GetServerBansAsync(null, clientId, null, null), Has.Count.EqualTo(1));
            });

            await pair.RunTicksSync(5);
            Assert.That(sPlayerManager.Sessions, Has.Length.EqualTo(0));
            Assert.That(!netMan.IsConnected);

            // Try to pardon a ban that does not exist
            await server.WaitPost(() => sConsole.ExecuteCommand($"pardon {missingBanId}")); // Amour edit

            // The existing ban is unaffected
            Assert.That(await sDatabase.GetServerBanAsync(null, clientId, null, null), Is.Not.Null);

            ban = await sDatabase.GetServerBanAsync(banId); // Amour edit
            Assert.Multiple(async () =>
            {
                Assert.That(ban, Is.Not.Null);
                Assert.That(await sDatabase.GetServerBansAsync(null, clientId, null, null), Has.Count.EqualTo(1));

                // Check that it matches
                Assert.That(ban.Id, Is.EqualTo(banId)); // Amour edit
                Assert.That(ban.UserId, Is.EqualTo(clientId));
                Assert.That(ban.BanTime.UtcDateTime - DateTime.UtcNow, Is.LessThanOrEqualTo(MarginOfError));
                Assert.That(ban.ExpirationTime, Is.Null); // Amour edit
                Assert.That(ban.Reason, Is.EqualTo(banReason));

                // Done through the console
                Assert.That(ban.BanningAdmin, Is.Null);
                Assert.That(ban.Unban, Is.Null);
            });

            // Pardon the actual ban
            await server.WaitPost(() => sConsole.ExecuteCommand($"pardon {banId}")); // Amour edit

            // No bans should be returned
            Assert.That(await sDatabase.GetServerBanAsync(null, clientId, null, null), Is.Null);

            // Direct id lookup returns a pardoned ban
            var pardonedBan = await sDatabase.GetServerBanAsync(banId); // Amour edit
            Assert.Multiple(async () =>
            {
                // Check that it matches
                Assert.That(pardonedBan, Is.Not.Null);

                // The list is still returned since that ignores pardons
                Assert.That(await sDatabase.GetServerBansAsync(null, clientId, null, null), Has.Count.EqualTo(1));

                Assert.That(pardonedBan.Id, Is.EqualTo(banId)); // Amour edit
                Assert.That(pardonedBan.UserId, Is.EqualTo(clientId));
                Assert.That(pardonedBan.BanTime.UtcDateTime - DateTime.UtcNow, Is.LessThanOrEqualTo(MarginOfError));
                Assert.That(pardonedBan.ExpirationTime, Is.Null); // Amour edit
                Assert.That(pardonedBan.Reason, Is.EqualTo(banReason));

                // Done through the console
                Assert.That(pardonedBan.BanningAdmin, Is.Null);

                Assert.That(pardonedBan.Unban, Is.Not.Null);
                Assert.That(pardonedBan.Unban.BanId, Is.EqualTo(banId)); // Amour edit

                // Done through the console
                Assert.That(pardonedBan.Unban.UnbanningAdmin, Is.Null);

                Assert.That(pardonedBan.Unban.UnbanTime.UtcDateTime - DateTime.UtcNow, Is.LessThanOrEqualTo(MarginOfError));
            });

            // Try to pardon it again
            await server.WaitPost(() => sConsole.ExecuteCommand($"pardon {banId}")); // Amour edit

            // Nothing changes
            Assert.Multiple(async () =>
            {
                // No bans should be returned
                Assert.That(await sDatabase.GetServerBanAsync(null, clientId, null, null), Is.Null);

                // Direct id lookup returns a pardoned ban
                Assert.That(await sDatabase.GetServerBanAsync(banId), Is.Not.Null); // Amour edit

                // The list is still returned since that ignores pardons
                Assert.That(await sDatabase.GetServerBansAsync(null, clientId, null, null), Has.Count.EqualTo(1));
            });

            // Reconnect client. Slightly faster than dirtying the pair.
            Assert.That(sPlayerManager.Sessions, Is.Empty);
            client.SetConnectTarget(server);
            await client.WaitPost(() => netMan.ClientConnect(null!, 0, null!));
            await pair.RunTicksSync(5);
            Assert.That(sPlayerManager.Sessions, Has.Length.EqualTo(1));

            await pair.CleanReturnAsync();
        }
    }
}
