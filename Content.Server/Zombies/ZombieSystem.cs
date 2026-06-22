// SPDX-FileCopyrightText: 2022 EmoGarbage404 <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Errant <35878406+dmnct@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022 metalgearsloth <comedian_vs_clown@hotmail.com>
// SPDX-FileCopyrightText: 2022 metalgearsloth <metalgearsloth@gmail.com>
// SPDX-FileCopyrightText: 2023 0x6273 <0x40@keemail.me>
// SPDX-FileCopyrightText: 2023 Alex Evgrashin <aevgrashin@yandex.ru>
// SPDX-FileCopyrightText: 2023 Doru991 <75124791+Doru991@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <drsmugleaf@gmail.com>
// SPDX-FileCopyrightText: 2023 Jezithyr <jezithyr@gmail.com>
// SPDX-FileCopyrightText: 2023 LankLTE <135308300+LankLTE@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Tom Leys <tom@crump-leys.com>
// SPDX-FileCopyrightText: 2023 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 corentt <36075110+corentt@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 keronshb <54602815+keronshb@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 nikthechampiongr <32041239+nikthechampiongr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 themias <89101928+themias@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Debug <49997488+DebugOk@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Jake Huxell <JakeHuxell@pm.me>
// SPDX-FileCopyrightText: 2024 Mr. 27 <45323883+Dutch-VanDerLinde@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Patrik Caes-Sayrs <heartofgoldfish@gmail.com>
// SPDX-FileCopyrightText: 2024 Tayrtahn <tayrtahn@gmail.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aidenkrz <28298836+Aidenkrz@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 CerberusWolfie <wb.johnb.willis@gmail.com>
// SPDX-FileCopyrightText: 2025 Errant <35878406+Errant-4@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 GoobBot <uristmchands@proton.me>
// SPDX-FileCopyrightText: 2025 IProduceWidgets <107586145+IProduceWidgets@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ScarKy0 <106310278+ScarKy0@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Ted Lukin <66275205+pheenty@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 UpAndLeaves <92269094+Alpha-Two@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 gluesniffler <159397573+gluesniffler@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 gluesniffler <linebarrelerenthusiast@gmail.com>
// SPDX-FileCopyrightText: 2025 pheenty <fedorlukin2006@gmail.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using System.Text;
using Content.Shared.HealthExaminable;
using Content.Shared.Speech;
using Content.Server._EinsteinEngines.Language;
using Content.Server._RW.Zombies;
using Content.Server.Actions;
using Content.Server.Body.Systems;
using Content.Server.Chat;
using Content.Server.Chat.Systems;
using Content.Server.Emoting.Systems;
using Content.Server.EUI;
using Content.Server.Roles;
using Content.Server.Speech.EntitySystems;
using Content.Shared._EinsteinEngines.Language.Components;
using Content.Shared._EinsteinEngines.Language.Events;
using Content.Shared._Shitmed.Damage;
using Content.Shared._Shitmed.Targeting;
using Content.Shared.Anomaly.Components;
using Content.Shared.Armor;
using Content.Shared.Bed.Sleep;
using Content.Shared.Blocking;
using Content.Shared.Cloning.Events;
using Content.Shared.Damage;
using Content.Shared.Humanoid;
using Content.Shared.Inventory;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.NPC.Prototypes;
using Content.Shared.Popups;
using Content.Shared.Roles;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Zombies;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Content.Shared.Body.Components;
using Content.Server.Temperature.Components;
using Content.Server.Body.Components;
using Content.Server.Atmos.Components;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.AnimalHusbandry;
using Content.Goobstation.Common.Traits;
using Content.Shared.Interaction.Components;
using Content.Shared.Weapons.Melee;
using Content.Shared.Hands.Components;
using Content.Shared.NPC.Components;
using Content.Server.NPC.HTN;
using Content.Shared.CombatMode.Pacification;
using Content.Server.Speech.Components;
using Content.Goobstation.Shared.Sprinting;
using Content.Shared.Prying.Components;

namespace Content.Server.Zombies
{
    public sealed partial class ZombieSystem : SharedZombieSystem
    {
        [Dependency] private readonly IGameTiming _timing = default!;
        [Dependency] private readonly IPrototypeManager _protoManager = default!;
        [Dependency] private readonly IRobustRandom _random = default!;
        [Dependency] private readonly BloodstreamSystem _bloodstream = default!;
        [Dependency] private readonly DamageableSystem _damageable = default!;
        [Dependency] private readonly ChatSystem _chat = default!;
        [Dependency] private readonly ActionsSystem _actions = default!;
        [Dependency] private readonly AutoEmoteSystem _autoEmote = default!;
        [Dependency] private readonly EmoteOnDamageSystem _emoteOnDamage = default!;
        [Dependency] private readonly MobStateSystem _mobState = default!;
        [Dependency] private readonly SharedPopupSystem _popup = default!;
        [Dependency] private readonly SharedRoleSystem _role = default!;
        [Dependency] private readonly LanguageSystem _language = default!;
        [Dependency] private readonly EuiManager _eui = default!; // RW edit
        [Dependency] private readonly MobThresholdSystem _mobThreshold = default!; // RW edit

        public readonly ProtoId<NpcFactionPrototype> Faction = "Zombie";

        // RW start
        private const int PendingZombieStagePale = PendingZombieComponent.PendingZombieStagePale;
        private const int PendingZombieStageSlowed = PendingZombieComponent.PendingZombieStageSlowed;
        private const int PendingZombieStageFinal = PendingZombieComponent.PendingZombieStageFinal;
        private const string PendingZombieLanguage = "Zombie";
        private const double PendingZombieStageSlowedSeconds = 180;
        private const double PendingZombieStageFinalSeconds = 300;
        private const double PendingZombieDeathSeconds = 420;
        private const double PendingZombieForcedTransformSeconds = 540;
        private static readonly TimeSpan PendingZombieTickInterval = TimeSpan.FromSeconds(1);
        private static readonly Color PendingZombieGreenSkin = new(0.45f, 0.51f, 0.29f);
        private static readonly string[] PendingZombieGroans =
        [
            "accent-words-zombie-1",
            "accent-words-zombie-2",
            "accent-words-zombie-3",
            "accent-words-zombie-4",
            "accent-words-zombie-5",
            "accent-words-zombie-6",
            "accent-words-zombie-7",
            "accent-words-zombie-8",
            "accent-words-zombie-9",
            "accent-words-zombie-10",
        ];
        // RW end

        public const SlotFlags ProtectiveSlots =
            SlotFlags.FEET |
            SlotFlags.HEAD |
            SlotFlags.EYES |
            SlotFlags.GLOVES |
            SlotFlags.MASK |
            SlotFlags.NECK |
            SlotFlags.INNERCLOTHING |
            SlotFlags.OUTERCLOTHING;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<ZombieComponent, EmoteEvent>(OnEmote, before:
                new[] { typeof(VocalSystem), typeof(BodyEmotesSystem) });

            SubscribeLocalEvent<ZombieComponent, MeleeHitEvent>(OnMeleeHit);
            SubscribeLocalEvent<ZombieComponent, MobStateChangedEvent>(OnMobState);
            SubscribeLocalEvent<ZombieComponent, CloningEvent>(OnZombieCloning);
            SubscribeLocalEvent<ZombieComponent, TryingToSleepEvent>(OnSleepAttempt);
            SubscribeLocalEvent<ZombieComponent, GetCharactedDeadIcEvent>(OnGetCharacterDeadIC);
            SubscribeLocalEvent<ZombieComponent, GetCharacterUnrevivableIcEvent>(OnGetCharacterUnrevivableIC);
            SubscribeLocalEvent<ZombieComponent, ComponentStartup>(OnStartup); // RW edit
            SubscribeLocalEvent<ZombieComponent, MindAddedMessage>(OnMindAdded);
            SubscribeLocalEvent<ZombieComponent, MindRemovedMessage>(OnMindRemoved);

            SubscribeLocalEvent<PendingZombieComponent, MapInitEvent>(OnPendingMapInit);
            SubscribeLocalEvent<PendingZombieComponent, BeforeRemoveAnomalyOnDeathEvent>(OnBeforeRemoveAnomalyOnDeath);

            SubscribeLocalEvent<IncurableZombieComponent, MapInitEvent>(OnPendingMapInit);

            SubscribeLocalEvent<ZombifyOnDeathComponent, MobStateChangedEvent>(OnDamageChanged);

            // Goob Edit - Prevent Zombies Speaking/Understanding Languages
            SubscribeLocalEvent<ZombieComponent, DetermineEntityLanguagesEvent>(OnLanguageApply);
            SubscribeLocalEvent<ZombieComponent, ComponentShutdown>(OnShutdown);

            // RW start
            SubscribeLocalEvent<PendingZombieComponent, ComponentShutdown>(OnPendingShutdown);
            SubscribeLocalEvent<PendingZombieComponent, DetermineEntityLanguagesEvent>(OnPendingLanguageApply);
            SubscribeLocalEvent<PendingZombieComponent, AccentGetEvent>(OnPendingAccentGet);
            SubscribeLocalEvent<PendingZombieComponent, HealthBeingExaminedEvent>(OnPendingHealthExamined);
            // RW end
        }

        private void OnBeforeRemoveAnomalyOnDeath(Entity<PendingZombieComponent> ent, ref BeforeRemoveAnomalyOnDeathEvent args)
        {
            // Pending zombies (e.g. infected non-zombies) do not remove their hosted anomaly on death.
            // Current zombies DO remove the anomaly on death.
            args.Cancelled = true;
        }

        private void OnPendingMapInit(EntityUid uid, IncurableZombieComponent component, MapInitEvent args)
        {
            _actions.AddAction(uid, ref component.Action, component.ZombifySelfActionPrototype);
            _faction.AddFaction(uid, Faction);

            if (HasComp<ZombieComponent>(uid) || HasComp<ZombieImmuneComponent>(uid))
                return;

            EnsureComp(uid, out PendingZombieComponent pendingComp);

            pendingComp.GracePeriod = pendingComp.MinInitialInfectedGrace == pendingComp.MaxInitialInfectedGrace
                ? pendingComp.MinInitialInfectedGrace
                : _random.Next(pendingComp.MinInitialInfectedGrace, pendingComp.MaxInitialInfectedGrace);
        }

        private void OnPendingMapInit(EntityUid uid, PendingZombieComponent component, MapInitEvent args)
        {
            if (_mobState.IsDead(uid))
            {
                ZombifyEntity(uid);
                return;
            }

            component.NextTick = _timing.CurTime + TimeSpan.FromSeconds(1f);
        }

        public override void Update(float frameTime)
        {
            base.Update(frameTime);
            var curTime = _timing.CurTime;

            // Hurt the living infected
            var query = EntityQueryEnumerator<PendingZombieComponent, DamageableComponent, MobStateComponent>();
            while (query.MoveNext(out var uid, out var comp, out var damage, out _))
            {
                // Process only once per second
                if (comp.NextTick > curTime)
                    continue;

                comp.NextTick = curTime + PendingZombieTickInterval;

                if (comp.GracePeriod > TimeSpan.Zero)
                {
                    comp.GracePeriod -= PendingZombieTickInterval;
                    if (comp.GracePeriod > TimeSpan.Zero)
                    {
                        continue;
                    }

                    comp.GracePeriod = TimeSpan.Zero;
                }

                // RW start
                comp.ActiveDuration += PendingZombieTickInterval;
                var seconds = comp.ActiveDuration.TotalSeconds;

                var newStage = seconds >= PendingZombieStageFinalSeconds
                    ? PendingZombieStageFinal
                    : seconds >= PendingZombieStageSlowedSeconds
                        ? PendingZombieStageSlowed
                        : PendingZombieStagePale;

                if (newStage != comp.CurrentStage)
                {
                    comp.CurrentStage = newStage;
                    ApplyPendingZombieStage(uid, comp, newStage);
                }
                else if (comp.CurrentStage == PendingZombieStageSlowed)
                {
                    UpdatePendingZombieStageTwoAppearance(uid, comp, seconds);
                }

                Dirty(uid, comp);

                if (comp.CurrentStage == PendingZombieStageFinal)
                {
                    if (seconds >= PendingZombieForcedTransformSeconds)
                    {
                        ZombifyEntity(uid);
                        continue;
                    }

                    if (_mobThreshold.TryGetThresholdForState(uid, MobState.Dead, out var deadThreshold)
                        && deadThreshold.HasValue)
                    {
                        var damageLeft = deadThreshold.Value - damage.TotalDamage;
                        if (damageLeft > 0)
                        {
                            var timeLeft = PendingZombieDeathSeconds - seconds;
                            if (timeLeft > 0)
                            {
                                var dmgTick = damageLeft.Float() / (float) timeLeft;
                                var dmgSpec = new DamageSpecifier();
                                dmgSpec.DamageDict.Add("Poison", dmgTick);
                                _damageable.TryChangeDamage(uid, dmgSpec, true, false, damage, targetPart: TargetBodyPart.All, splitDamage: SplitDamageBehavior.SplitEnsureAll);
                            }
                        }
                    }
                }
                // RW end
            }

            // Heal the zombified
            var zombQuery = EntityQueryEnumerator<ZombieComponent, DamageableComponent, MobStateComponent>();
            while (zombQuery.MoveNext(out var uid, out var comp, out var damage, out var mobState))
            {
                // Process only once per second
                if (comp.NextTick + TimeSpan.FromSeconds(1) > curTime)
                    continue;

                comp.NextTick = curTime;

                if (_mobState.IsDead(uid, mobState))
                    continue;

                var multiplier = _mobState.IsCritical(uid, mobState)
                    ? comp.PassiveHealingCritMultiplier
                    : 1f;

                // Gradual healing for living zombies.
                _damageable.TryChangeDamage(uid,
                    comp.PassiveHealing * multiplier,
                    true,
                    false,
                    damage,
                    ignoreBlockers: true, // Shitmed Change
                    targetPart: TargetBodyPart.All, // Shitmed Change
                    splitDamage: SplitDamageBehavior.SplitEnsureAll); // Shitmed Change
            }
        }

        private void OnSleepAttempt(EntityUid uid, ZombieComponent component, ref TryingToSleepEvent args)
        {
            args.Cancelled = true;
        }

        private void OnGetCharacterDeadIC(EntityUid uid, ZombieComponent component, ref GetCharactedDeadIcEvent args)
        {
            args.Dead = true;
        }

        private void OnGetCharacterUnrevivableIC(EntityUid uid, ZombieComponent component, ref GetCharacterUnrevivableIcEvent args)
        {
            args.Unrevivable = true;
        }

        private void OnStartup(EntityUid uid, ZombieComponent component, ComponentStartup args)
        {
            if (component.EmoteSoundsId == null
                || TerminatingOrDeleted(uid)) // Goob Change
                return;

            // Goobstation Change Start
            var comp = EnsureComp<LanguageSpeakerComponent>(uid); // Ensure they can speak language before adding language.
            if (!string.IsNullOrEmpty(component.ForcedLanguage)) // Should never be false, but security either way.
                comp.CurrentLanguage = component.ForcedLanguage;
            _language.UpdateEntityLanguages(uid);
            // Goobstation Change End
        }

        private void OnEmote(EntityUid uid, ZombieComponent component, ref EmoteEvent args)
        {
            // always play zombie emote sounds and ignore others
            if (args.Handled)
                return;

            _protoManager.TryIndex(component.EmoteSoundsId, out var sounds);

            args.Handled = _chat.TryPlayEmoteSound(uid, sounds, args.Emote);
        }

        private void OnMobState(EntityUid uid, ZombieComponent component, MobStateChangedEvent args)
        {
            if (args.NewMobState == MobState.Alive)
            {
                // Groaning when damaged
                EnsureComp<EmoteOnDamageComponent>(uid);
                _emoteOnDamage.AddEmote(uid, "Scream");

                // Random groaning
                EnsureComp<AutoEmoteComponent>(uid);
                _autoEmote.AddEmote(uid, "ZombieGroan");
            }
            else
            {
                // Stop groaning when damaged
                _emoteOnDamage.RemoveEmote(uid, "Scream");

                // Stop random groaning
                _autoEmote.RemoveEmote(uid, "ZombieGroan");
            }
        }

        private bool IsUserBlocking(BlockingUserComponent? component) // Goobstation
        {
            return (TryComp<BlockingComponent>(component?.BlockingItem, out var blockComp) && blockComp.IsBlocking);
        }

        private float GetZombieInfectionChance(EntityUid uid, ZombieComponent zombieComponent)
        {
            var chance = zombieComponent.BaseZombieInfectionChance;

            var armorEv = new CoefficientQueryEvent(ProtectiveSlots);
            RaiseLocalEvent(uid, armorEv);
            foreach (var resistanceEffectiveness in zombieComponent.ResistanceEffectiveness.DamageDict)
            {
                if (armorEv.DamageModifiers.Coefficients.TryGetValue(resistanceEffectiveness.Key, out var coefficient))
                {
                    // Scale the coefficient by the resistance effectiveness, very descriptive I know
                    // For example. With 30% slash resist (0.7 coeff), but only a 60% resistance effectiveness for slash,
                    // you'll end up with 1 - (0.3 * 0.6) = 0.82 coefficient, or a 18% resistance
                    var adjustedCoefficient = 1 - ((1 - coefficient) * resistanceEffectiveness.Value.Float());
                    chance *= adjustedCoefficient;
                }
            }

            var zombificationResistanceEv = new ZombificationResistanceQueryEvent(ProtectiveSlots);
            RaiseLocalEvent(uid, zombificationResistanceEv);
            chance *= zombificationResistanceEv.TotalCoefficient;

            return MathF.Max(chance, zombieComponent.MinZombieInfectionChance);
        }

        private void OnMeleeHit(EntityUid uid, ZombieComponent component, MeleeHitEvent args)
        {
            if (!TryComp<ZombieComponent>(args.User, out _))
                return;

            if (!args.HitEntities.Any())
                return;

            foreach (var entity in args.HitEntities)
            {
                if (args.User == entity)
                    continue;

                if (!TryComp<MobStateComponent>(entity, out var mobState))
                    continue;

                if (TryComp<BlockingUserComponent>(entity, out var blockingUser) && IsUserBlocking(blockingUser)) // Goobstation edit - prevents infection if user is actively blocking
                    return;

                if (HasComp<ZombieComponent>(entity) || HasComp<InitialInfectedComponent>(entity)) // Goobstation edit - prevent zombies from damaging IIs
                {
                    args.BonusDamage = -args.BaseDamage;
                }
                else
                {
                    if (!HasComp<ZombieImmuneComponent>(entity) && !HasComp<NonSpreaderZombieComponent>(args.User) && _random.Prob(GetZombieInfectionChance(entity, component)))
                    {
                        EnsureComp<PendingZombieComponent>(entity);
                        EnsureComp<ZombifyOnDeathComponent>(entity);
                    }
                }

                if (_mobState.IsIncapacitated(entity, mobState) && !HasComp<ZombieComponent>(entity) && !HasComp<ZombieImmuneComponent>(entity) && !HasComp<InitialInfectedComponent>(entity)) // Goobstation edit - prevent zombies from damaging IIs
                {
                    ZombifyEntity(entity);
                    args.BonusDamage = -args.BaseDamage;
                }
                else if (mobState.CurrentState == MobState.Alive) //heals when zombies bite live entities
                {
                    _damageable.TryChangeDamage(uid, component.HealingOnBite, true, false);
                }
            }
        }

        private void OverrideComp<T>(EntityUid target, EntityUid source) where T : IComponent // Goob, for below function
        {
            if (!TryComp(source, out T? toCopy))
            {
                RemComp<T>(target);
                return;
            }

            CopyComp<T>(source, target, toCopy);
        }

        // GOOB START - completely rewrote function to actually work now
        /// <summary>
        ///     This is the function to call if you want to unzombify an entity.
        /// </summary>
        /// <param name="source">the entity having the ZombieComponent</param>
        /// <param name="target">the entity you want to unzombify (different from source in case of cloning, for example)</param>
        /// <param name="zombieComp"></param>
        /// <remarks>
        ///     goob note: this now restores the character pretty much completely*, upstream is only skin/eye color
        ///     *not without sacrifices to sane code
        /// </remarks>
        public bool UnZombify(EntityUid source, EntityUid target, ZombieComponent? zombieComp) // This function is really stupid but it works
        {
            if (!Resolve(source, ref zombieComp))
                return false;

            RemComp<ZombieComponent>(target);
            if (zombieComp.BeforeZombificationReferenceEnt is not { } reference)
            {
                Log.Error($"Failed to properly reverse zombification of entity \"{ToPrettyString(target)}\"!");
                return false;
            }

            //OverrideComp<HumanoidAppearanceComponent>(target, referenceEnt.Value); // For some reason, this does not work properly in copying appearance
            if (TryComp(target, out HumanoidAppearanceComponent? targetHumanoidAppearance)
                && TryComp(reference, out HumanoidAppearanceComponent? referenceHumanoidAppearance))
            {
                _humanoidAppearance.CloneAppearance(reference, target, referenceHumanoidAppearance, targetHumanoidAppearance);
            }

            // Override components that zombification tampers with reference clone components.
            // - OverrideComp() just removes them if reference clone doesn't have them, otherwise copies component.
            // - This is kind of ass but what can we do about it, at least this actually mostly fixes it unlike upstream UnZombify().
            OverrideComp<DamageableComponent>(target, reference);
            OverrideComp<TemperatureComponent>(target, reference);
            OverrideComp<SprinterComponent>(target, reference);

            OverrideComp<RespiratorComponent>(target, reference);
            OverrideComp<BarotraumaComponent>(target, reference);
            OverrideComp<HungerComponent>(target, reference);
            OverrideComp<ThirstComponent>(target, reference);
            OverrideComp<ReproductiveComponent>(target, reference);
            OverrideComp<ReproductivePartnerComponent>(target, reference);
            OverrideComp<LegsParalyzedComponent>(target, reference);
            OverrideComp<ComplexInteractionComponent>(target, reference);

            OverrideComp<MeleeWeaponComponent>(target, reference);

            OverrideComp<EmoteOnDamageComponent>(target, reference);
            OverrideComp<AutoEmoteComponent>(target, reference);
            OverrideComp<HandsComponent>(target, reference);
            OverrideComp<NpcFactionMemberComponent>(target, reference);
            OverrideComp<HTNComponent>(target, reference);
            OverrideComp<PacifiedComponent>(target, reference);
            OverrideComp<ReplacementAccentComponent>(target, reference);
            OverrideComp<PryingComponent>(target, reference);
            if (TryComp(reference, out BloodstreamComponent? referenceBloodstream))
            {
                EnsureComp<BloodstreamComponent>(target);
                _bloodstream.ChangeBloodReagent(target, referenceBloodstream.BloodReagent);
                _bloodstream.SetBloodLossThreshold(target, referenceBloodstream.BloodlossThreshold);
            }

            // seems to fix no hand being selected
            if (HasComp<HandsComponent>(target)
                && _hands.TryGetEmptyHand(target, out string? emptyHand))
            {
                _hands.SetActiveHand(target, emptyHand);
            }

            // gotta make sure it tells them theyre no longer antag
            var hasMind = _mind.TryGetMind(target, out var mindId, out var mind);
            if (hasMind)
                _role.MindRemoveRole(mindId, "MindRoleZombie");

            _nameMod.RefreshNameModifiers(target);
            _identity.QueueIdentityUpdate(target);

            // free up the reference clone
            QueueDel(reference);

            return true;
        }
        // GOOB END

        private void OnZombieCloning(Entity<ZombieComponent> ent, ref CloningEvent args)
        {
            UnZombify(ent.Owner, args.CloneUid, ent.Comp);
        }

        // Make sure players that enter a zombie (for example via a ghost role or the mind swap spell) count as an antagonist.
        private void OnMindAdded(Entity<ZombieComponent> ent, ref MindAddedMessage args)
        {
            if (!_role.MindHasRole<ZombieRoleComponent>(args.Mind))
                _role.MindAddRole(args.Mind, "MindRoleZombie", mind: args.Mind.Comp);

            // RW start
            if (_player.TryGetSessionById(args.Mind.Comp.UserId, out var session))
                _eui.OpenEui(new ZombieRoleEui(), session);
            // RW end
        }

        // Remove the role when getting cloned, getting gibbed and borged, or leaving the body via any other method.
        private void OnMindRemoved(Entity<ZombieComponent> ent, ref MindRemovedMessage args)
        {
            _role.MindRemoveRole<ZombieRoleComponent>((args.Mind.Owner, args.Mind.Comp));
        }

        #region Goob Language Changes

        /// <summary>
        ///     This forces the languages to reset and apply only the current language for the entity based on Zombie Component.
        /// </summary>
        private void OnLanguageApply(Entity<ZombieComponent> ent, ref DetermineEntityLanguagesEvent args)
        {
            if (ent.Comp.LifeStage is ComponentLifeStage.Removing
                or ComponentLifeStage.Stopping
                or ComponentLifeStage.Stopped)
                return;

            // Clear the languages and then apply the forced language.
            args.SpokenLanguages.Clear();
            args.UnderstoodLanguages.Clear();
            args.SpokenLanguages.Add(ent.Comp.ForcedLanguage);
            args.UnderstoodLanguages.Add(ent.Comp.ForcedLanguage);
        }

        // When comp is removed, reset languages.
        private void OnShutdown(Entity<ZombieComponent> ent, ref ComponentShutdown args)
        {
            if (TerminatingOrDeleted(ent))
                return;

            _language.UpdateEntityLanguages(ent.Owner); // This uses ent.Owner because UpdateEntityLanguages checks for <LanguageSpeakerComponent>.
        }

        #endregion

        // RW start
        private void OnPendingShutdown(EntityUid uid, PendingZombieComponent component, ComponentShutdown args)
        {
            if (TerminatingOrDeleted(uid) || HasComp<ZombieComponent>(uid))
                return;

            if (component.OriginalSkinColor is { } originalColor && TryComp<HumanoidAppearanceComponent>(uid, out var humanoid))
            {
                _humanoidAppearance.SetSkinColor(uid, originalColor, verify: false, humanoid: humanoid);
            }

            if (component.CurrentStage >= PendingZombieStageSlowed)
                _movementSpeedModifier.RefreshMovementSpeedModifiers(uid);

            if (component.CurrentStage >= PendingZombieStageFinal)
                _language.UpdateEntityLanguages(uid);
        }

        private void OnPendingLanguageApply(EntityUid uid, PendingZombieComponent component, ref DetermineEntityLanguagesEvent args)
        {
            if (HasComp<ZombieComponent>(uid))
                return;

            if (component.CurrentStage == PendingZombieStageFinal)
            {
                args.SpokenLanguages.Remove(PendingZombieLanguage);
                args.UnderstoodLanguages.Add(PendingZombieLanguage);
            }
        }

        private void OnPendingAccentGet(EntityUid uid, PendingZombieComponent component, AccentGetEvent args)
        {
            if (HasComp<ZombieComponent>(uid))
                return;

            if (component.CurrentStage >= PendingZombieStageSlowed)
            {
                var message = args.Message;
                var words = message.Split(' ');
                var replacedAny = false;

                for (var i = 0; i < words.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(words[i]))
                        continue;

                    if (_random.Prob(0.25f))
                    {
                        var groanKey = _random.Pick(PendingZombieGroans);
                        words[i] = Loc.GetString(groanKey);
                        replacedAny = true;
                    }
                }

                if (replacedAny)
                {
                    message = string.Join(" ", words);
                }

                args.Message = Accentuate(message, 0.8f);
            }
        }

        private void OnPendingHealthExamined(EntityUid uid, PendingZombieComponent component, ref HealthBeingExaminedEvent args)
        {
            if (component.CurrentStage == PendingZombieStagePale)
            {
                args.Message.PushNewline();
                args.Message.AddMarkupOrThrow(Loc.GetString("bloodstream-component-looks-pale", ("target", uid)));
            }
            else if (component.CurrentStage == PendingZombieStageSlowed)
            {
                args.Message.PushNewline();
                args.Message.AddMarkupOrThrow(Loc.GetString("zombie-infection-looks-greenish", ("target", uid)));
            }
        }

        private void ApplyPendingZombieStage(EntityUid uid, PendingZombieComponent component, int newStage)
        {
            if (TryComp<HumanoidAppearanceComponent>(uid, out var humanoid))
            {
                component.OriginalSkinColor ??= humanoid.SkinColor;
                var originalColor = component.OriginalSkinColor.Value;

                var skinColor = newStage switch
                {
                    PendingZombieStagePale => Color.InterpolateBetween(originalColor, Color.White, 0.4f),
                    PendingZombieStageSlowed => GetPendingZombieStageTwoSkinColor(component, 0),
                    PendingZombieStageFinal => PendingZombieGreenSkin,
                    _ => humanoid.SkinColor,
                };

                _humanoidAppearance.SetSkinColor(uid, skinColor, verify: false, humanoid: humanoid);
            }

            if (newStage == PendingZombieStagePale)
            {
                _popup.PopupEntity(Loc.GetString("zombie-infection-stage-pale"), uid, uid);
            }
            else if (newStage == PendingZombieStageSlowed)
            {
                _popup.PopupEntity(Loc.GetString("zombie-infection-stage-slowed"), uid, uid);
                _movementSpeedModifier.RefreshMovementSpeedModifiers(uid);
            }
            else if (newStage == PendingZombieStageFinal)
            {
                _popup.PopupEntity(Loc.GetString("zombie-infection-stage-final"), uid, uid);
                EnsureComp<LanguageSpeakerComponent>(uid);
                _language.UpdateEntityLanguages(uid);
            }
        }

        private void UpdatePendingZombieStageTwoAppearance(EntityUid uid, PendingZombieComponent component, double seconds)
        {
            if (!TryComp<HumanoidAppearanceComponent>(uid, out var humanoid))
                return;

            var stageProgress = Math.Clamp(
                (seconds - PendingZombieStageSlowedSeconds) / (PendingZombieStageFinalSeconds - PendingZombieStageSlowedSeconds),
                0,
                1);

            _humanoidAppearance.SetSkinColor(uid, GetPendingZombieStageTwoSkinColor(component, stageProgress), verify: false, humanoid: humanoid);
        }

        private Color GetPendingZombieStageTwoSkinColor(PendingZombieComponent component, double stageProgress)
        {
            var originalColor = component.OriginalSkinColor ?? Color.White;
            var paleColor = Color.InterpolateBetween(originalColor, Color.White, 0.4f);
            return Color.InterpolateBetween(paleColor, PendingZombieGreenSkin, (float) stageProgress);
        }

        private string Accentuate(string message, float scale)
        {
            var sb = new StringBuilder();

            foreach (var character in message)
            {
                if (_random.Prob(scale / 3f))
                {
                    var lower = char.ToLowerInvariant(character);
                    var newString = lower switch
                    {
                        'o' => "u",
                        's' => "ch",
                        'a' => "ah",
                        'u' => "oo",
                        'c' => "k",
                        // Russian
                        'о' => "у",
                        'с' => "ш",
                        'а' => "ах",
                        'у' => "уу",
                        'и' => "ы",
                        'е' => "ээ",
                        _ => $"{character}",
                    };

                    sb.Append(newString);
                }

                if (_random.Prob(scale / 20f))
                {
                    if (character == ' ')
                    {
                        sb.Append(Loc.GetString("slur-accent-confused"));
                    }
                    else if (character == '.')
                    {
                        sb.Append(' ');
                        sb.Append(Loc.GetString("slur-accent-burp"));
                    }
                }

                if (!_random.Prob(scale * 3 / 20))
                {
                    sb.Append(character);
                    continue;
                }

                var next = _random.Next(1, 3) switch
                {
                    1 => "'",
                    2 => $"{character}{character}",
                    _ => $"{character}{character}{character}",
                };

                sb.Append(next);
            }

            return sb.ToString();
        }
        // RW end
    }
}
