using Content.Server.Administration.Managers;
using Content.Server.Administration.Logs;
using Content.Server.Chat.Managers;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Rules;
using Content.Server.StationEvents;
using Content.Server.StationEvents.Components;
using Content.Shared.Administration;
using Content.Shared.Eui;
using Content.Shared._RW.GameFlowControl;
using Content.Shared.GameTicking.Components;
using Content.Shared.Database;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System.Linq;

namespace Content.Server._RW.GameFlowControl;

public sealed class GameFlowControlSystem : EntitySystem
{
    [Dependency] private readonly IAdminManager _adminManager = default!;
    [Dependency] private readonly IAdminLogManager _adminLogger = default!;
    [Dependency] private readonly IChatManager _chatManager = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly GameTicker _gameTicker = default!;
    [Dependency] private readonly IComponentFactory _componentFactory = default!;

    private ICommonSession? _occupier;
    public GameFlowControlEui? ActiveEui;
    private float _updateTimer = 0f; // RW

    private readonly List<PendingRule> _pendingRules = new();

    private struct PendingRule
    {
        public EntityUid Entity;
        public string RuleId;
        public TimeSpan StartTime;
    }

    public override void Initialize()
    {
        base.Initialize();

        SubscribeNetworkEvent<RequestGameFlowControlStateEvent>(OnRequestState);
    }

    public bool IsOccupied() => _occupier != null;

    public string? GetOccupierName() => _occupier?.Name;

    public bool TryOccupy(ICommonSession session)
    {
        if (_occupier != null && _occupier != session)
            return false;

        if (_occupier == session)
            return true;

        _occupier = session;

        // Alert admins
        var alertMsg = Loc.GetString("game-flow-control-occupied", ("username", session.Name));
        _chatManager.SendAdminAlert(alertMsg);

        // Broadcast new state to all clients
        RaiseNetworkEvent(new GameFlowControlStateEvent(session.Name));

        return true;
    }

    public void ReleaseControl(ICommonSession session)
    {
        if (_occupier != session)
            return;

        _occupier = null;
        ActiveEui = null;

        // Alert admins
        var alertMsg = Loc.GetString("game-flow-control-released");
        _chatManager.SendAdminAlert(alertMsg);

        // Broadcast released state
        RaiseNetworkEvent(new GameFlowControlStateEvent(null));

        // Immediately approve all pending rules so they aren't stuck
        ApproveAllPending();
    }

    private void OnRequestState(RequestGameFlowControlStateEvent ev, EntitySessionEventArgs args)
    {
        // Send current occupier state back to the requesting client
        RaiseNetworkEvent(new GameFlowControlStateEvent(_occupier?.Name), args.SenderSession.Channel);
    }

    public void AddPendingRule(EntityUid ruleUid, string ruleId)
    {
        if (_pendingRules.Any(r => r.Entity == ruleUid))
            return;

        _pendingRules.Add(new PendingRule
        {
            Entity = ruleUid,
            RuleId = ruleId,
            StartTime = _gameTiming.CurTime
        });

        ActiveEui?.StateDirty();
    }

    public void ApproveRule(EntityUid ruleUid)
    {
        var index = _pendingRules.FindIndex(r => r.Entity == ruleUid);
        if (index < 0)
            return;

        var rule = _pendingRules[index];
        _pendingRules.RemoveAt(index);

        if (Exists(ruleUid))
        {
            EnsureComp<GameFlowControlApprovedComponent>(ruleUid);
            RemComp<PendingApprovalRuleComponent>(ruleUid);

            // Send admin alert now that it's approved and actually running
            Logger.GetSawmill("stationevents").Info($"Added game rule {ToPrettyString(ruleUid)}");
            _adminLogger.Add(LogType.EventStarted, $"Added game rule {ToPrettyString(ruleUid)}");
            var str = Loc.GetString("station-event-system-run-event", ("eventName", ToPrettyString(ruleUid)));
            _chatManager.SendAdminAlert(str);
            Log.Info(str);

            // Defer: Raise GameRuleAddedEvent now that the rule is approved
            var ruleData = EnsureComp<GameRuleComponent>(ruleUid);
            var addedEv = new GameRuleAddedEvent(ruleUid, rule.RuleId);
            RaiseLocalEvent(ruleUid, ref addedEv, true);

            // Trigger announcement and audio
            var approvedEv = new GameFlowControlRuleApprovedEvent();
            RaiseLocalEvent(ruleUid, ref approvedEv, true);

            // Start it
            _gameTicker.StartGameRule(ruleUid);
        }

        ActiveEui?.StateDirty();
    }

    public void DenyRule(EntityUid ruleUid)
    {
        var index = _pendingRules.FindIndex(r => r.Entity == ruleUid);
        if (index < 0)
            return;

        var rule = _pendingRules[index];
        _pendingRules.RemoveAt(index);

        if (Exists(ruleUid))
        {
            RemComp<PendingApprovalRuleComponent>(ruleUid);
            _gameTicker.RemovePendingGameRule(rule.RuleId);
            QueueDel(ruleUid);
        }

        ActiveEui?.StateDirty();
    }

    private void ApproveAllPending()
    {
        var copy = _pendingRules.ToList();
        _pendingRules.Clear();

        foreach (var rule in copy)
        {
            if (!Exists(rule.Entity))
                continue;

            EnsureComp<GameFlowControlApprovedComponent>(rule.Entity);
            RemComp<PendingApprovalRuleComponent>(rule.Entity);

            // Send admin alert now that it's approved and actually running
            Logger.GetSawmill("stationevents").Info($"Added game rule {ToPrettyString(rule.Entity)}");
            _adminLogger.Add(LogType.EventStarted, $"Added game rule {ToPrettyString(rule.Entity)}");
            var str = Loc.GetString("station-event-system-run-event", ("eventName", ToPrettyString(rule.Entity)));
            _chatManager.SendAdminAlert(str);
            Log.Info(str);

            // Defer: Raise GameRuleAddedEvent now that the rule is approved
            var ruleData = EnsureComp<GameRuleComponent>(rule.Entity);
            var addedEv = new GameRuleAddedEvent(rule.Entity, rule.RuleId);
            RaiseLocalEvent(rule.Entity, ref addedEv, true);

            var approvedEv = new GameFlowControlRuleApprovedEvent();
            RaiseLocalEvent(rule.Entity, ref approvedEv, true);

            _gameTicker.StartGameRule(rule.Entity);
        }
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        // RW start
        if (ActiveEui != null)
        {
            _updateTimer += frameTime;
            if (_updateTimer >= 1.0f)
            {
                _updateTimer = 0f;
                ActiveEui.StateDirty();
            }
        }
        // RW end

        if (_pendingRules.Count == 0)
            return;

        var curTime = _gameTiming.CurTime;
        var toApprove = new List<EntityUid>();
        var toRemove = new List<int>();

        for (var i = 0; i < _pendingRules.Count; i++)
        {
            var rule = _pendingRules[i];
            if (!Exists(rule.Entity) || Deleted(rule.Entity))
            {
                toRemove.Add(i);
                continue;
            }

            var elapsed = (curTime - rule.StartTime).TotalSeconds;
            if (elapsed >= 30.0)
            {
                toApprove.Add(rule.Entity);
            }
        }

        // Clean up nonexistent rule entities
        if (toRemove.Count > 0)
        {
            foreach (var idx in toRemove.OrderByDescending(x => x))
            {
                _pendingRules.RemoveAt(idx);
            }
            ActiveEui?.StateDirty();
        }

        // Approve timed-out rule entities
        if (toApprove.Count > 0)
        {
            foreach (var ruleUid in toApprove)
            {
                ApproveRule(ruleUid);
            }
            ActiveEui?.StateDirty();
        }
    }

    public void TriggerRule(string ruleId)
    {
        var ruleUid = _gameTicker.AddGameRule(ruleId);
        
        // If it was added to pending, don't approve immediately. Just dirty state.
        if (!_pendingRules.Any(r => r.Entity == ruleUid))
        {
            // If for some reason it wasn't added to pending, start it normally.
            EnsureComp<GameFlowControlApprovedComponent>(ruleUid);
            _gameTicker.StartGameRule(ruleUid);
        }

        ActiveEui?.StateDirty();
    }

    public void SetInterval(float min, float max, float timeLeft)
    {
        // 1. Basic Scheduler
        var basicQuery = EntityQueryEnumerator<BasicStationEventSchedulerComponent>();
        while (basicQuery.MoveNext(out var basic))
        {
            basic.MinMaxEventTiming = new Content.Shared.Destructible.Thresholds.MinMax((int) min, (int) max);
            basic.TimeUntilNextEvent = timeLeft;
        }

        // 2. Ramping Scheduler
        var rampingQuery = EntityQueryEnumerator<RampingStationEventSchedulerComponent>();
        while (rampingQuery.MoveNext(out var ramping))
        {
            ramping.TimeUntilNextEvent = timeLeft;
        }

        // 3. SecretPlus Scheduler (reflection bypass due to assembly boundaries)
        if (_componentFactory.TryGetRegistration("SecretPlus", out var reg))
        {
            var components = EntityManager.GetAllComponents(reg.Type);
            foreach (var (uid, comp) in components)
            {
                var minField = reg.Type.GetField("EventIntervalMin");
                var maxField = reg.Type.GetField("EventIntervalMax");
                var nextField = reg.Type.GetField("TimeNextEvent");

                if (minField != null) minField.SetValue(comp, TimeSpan.FromSeconds(min));
                if (maxField != null) maxField.SetValue(comp, TimeSpan.FromSeconds(max));
                if (nextField != null) nextField.SetValue(comp, _gameTiming.CurTime + TimeSpan.FromSeconds(timeLeft));
            }
        }

        // Send alert
        var alertMsg = Loc.GetString("game-flow-control-interval-changed",
            ("username", _occupier?.Name ?? ""),
            ("min", (int) min),
            ("max", (int) max),
            ("timeLeft", (int) timeLeft));
        _chatManager.SendAdminAlert(alertMsg);

        ActiveEui?.StateDirty();
    }

    public void SetChaos(float chaos)
    {
        // SecretPlus Scheduler chaos score edit (reflection bypass)
        if (_componentFactory.TryGetRegistration("SecretPlus", out var reg))
        {
            var components = EntityManager.GetAllComponents(reg.Type);
            foreach (var (uid, comp) in components)
            {
                var chaosField = reg.Type.GetField("ChaosScore");
                if (chaosField != null)
                {
                    chaosField.SetValue(comp, chaos);
                }
            }
        }

        // Send alert
        var alertMsg = Loc.GetString("game-flow-control-chaos-changed",
            ("username", _occupier?.Name ?? ""),
            ("chaos", (int) chaos));
        _chatManager.SendAdminAlert(alertMsg);

        ActiveEui?.StateDirty();
    }

    public void PopulateEuiState(GameFlowControlEuiState state)
    {
        state.OccupierName = _occupier?.Name;

        // Find active scheduler details
        state.ActiveScheduler = Loc.GetString("game-flow-control-no-scheduler");
        state.MinInterval = 0;
        state.MaxInterval = 0;
        state.TimeUntilNext = 0;
        state.ChaosScore = 0;

        var basicQuery = EntityQueryEnumerator<BasicStationEventSchedulerComponent, GameRuleComponent>();
        while (basicQuery.MoveNext(out var uid, out var basic, out var rule))
        {
            if (!_gameTicker.IsGameRuleActive(uid, rule))
                continue;
            state.ActiveScheduler = "Basic";
            state.MinInterval = basic.MinMaxEventTiming.Min;
            state.MaxInterval = basic.MinMaxEventTiming.Max;
            state.TimeUntilNext = basic.TimeUntilNextEvent;
            break;
        }

        var rampingQuery = EntityQueryEnumerator<RampingStationEventSchedulerComponent, GameRuleComponent>();
        while (rampingQuery.MoveNext(out var uid, out var ramping, out var rule))
        {
            if (!_gameTicker.IsGameRuleActive(uid, rule))
                continue;
            state.ActiveScheduler = "Ramping";
            state.MinInterval = 0;
            state.MaxInterval = 0;
            state.TimeUntilNext = ramping.TimeUntilNextEvent;
            break;
        }

        // SecretPlus Scheduler state population (reflection bypass)
        if (_componentFactory.TryGetRegistration("SecretPlus", out var reg))
        {
            var components = EntityManager.GetAllComponents(reg.Type);
            foreach (var (uid, comp) in components)
            {
                if (TryComp<GameRuleComponent>(uid, out var rule) && !_gameTicker.IsGameRuleActive(uid, rule))
                    continue;

                state.ActiveScheduler = "SecretPlus";

                var minField = reg.Type.GetField("EventIntervalMin")?.GetValue(comp) as TimeSpan?;
                var maxField = reg.Type.GetField("EventIntervalMax")?.GetValue(comp) as TimeSpan?;
                var nextField = reg.Type.GetField("TimeNextEvent")?.GetValue(comp) as TimeSpan?;
                var chaosField = reg.Type.GetField("ChaosScore")?.GetValue(comp) as float?;

                state.MinInterval = minField != null ? (float)minField.Value.TotalSeconds : 0f;
                state.MaxInterval = maxField != null ? (float)maxField.Value.TotalSeconds : 0f;
                state.TimeUntilNext = nextField != null ? (float)Math.Max(0.0, (nextField.Value - _gameTiming.CurTime).TotalSeconds) : 0f;
                state.ChaosScore = chaosField ?? 0f;
                break;
            }
        }

        // Populate pending rules list
        state.PendingRules.Clear();
        var curTime = _gameTiming.CurTime;
        foreach (var rule in _pendingRules)
        {
            if (!Exists(rule.Entity) || Deleted(rule.Entity))
                continue;
            var elapsed = (curTime - rule.StartTime).TotalSeconds;
            var timeLeft = (float)Math.Max(0.0, 30.0 - elapsed);
            state.PendingRules.Add(new PendingRuleData(GetNetEntity(rule.Entity), rule.RuleId, timeLeft));
        }

        // Populate past rules list
        state.PastRules.Clear();
        foreach (var past in _gameTicker.AllPreviousGameRules)
        {
            state.PastRules.Add(new PastRuleData(past.Item1.ToString(@"hh\:mm\:ss"), past.Item2));
        }

        // Populate all rules list
        state.AllRules.Clear();
        foreach (var proto in _gameTicker.GetAllGameRulePrototypes())
        {
            state.AllRules.Add(proto.ID);
        }
        state.AllRules.Sort();
    }
}
