using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared._RW.GameFlowControl;

[NetSerializable, Serializable]
public struct PendingRuleData
{
    public NetEntity Entity;
    public string RuleId;
    public float TimeLeft;

    public PendingRuleData(NetEntity entity, string ruleId, float timeLeft)
    {
        Entity = entity;
        RuleId = ruleId;
        TimeLeft = timeLeft;
    }
}

[NetSerializable, Serializable]
public struct PastRuleData
{
    public string Time;
    public string RuleId;

    public PastRuleData(string time, string ruleId)
    {
        Time = time;
        RuleId = ruleId;
    }
}

[NetSerializable, Serializable]
public sealed class GameFlowControlEuiState : EuiStateBase
{
    public string? OccupierName;
    public string ActiveScheduler = "";
    public float MinInterval;
    public float MaxInterval;
    public float TimeUntilNext;
    public float ChaosScore;
    public List<PendingRuleData> PendingRules = new();
    public List<PastRuleData> PastRules = new();
    public List<string> AllRules = new();
}
