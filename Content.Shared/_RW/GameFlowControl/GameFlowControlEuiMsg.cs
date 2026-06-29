using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared._RW.GameFlowControl;

[NetSerializable, Serializable]
public sealed class SetIntervalMsg : EuiMessageBase
{
    public float Min;
    public float Max;
    public float TimeLeft;

    public SetIntervalMsg(float min, float max, float timeLeft)
    {
        Min = min;
        Max = max;
        TimeLeft = timeLeft;
    }
}

[NetSerializable, Serializable]
public sealed class SetChaosMsg : EuiMessageBase
{
    public float Score;

    public SetChaosMsg(float score)
    {
        Score = score;
    }
}

[NetSerializable, Serializable]
public sealed class ApproveRuleMsg : EuiMessageBase
{
    public NetEntity Entity;

    public ApproveRuleMsg(NetEntity entity)
    {
        Entity = entity;
    }
}

[NetSerializable, Serializable]
public sealed class DenyRuleMsg : EuiMessageBase
{
    public NetEntity Entity;

    public DenyRuleMsg(NetEntity entity)
    {
        Entity = entity;
    }
}

[NetSerializable, Serializable]
public sealed class TriggerRuleMsg : EuiMessageBase
{
    public string RuleId;

    public TriggerRuleMsg(string ruleId)
    {
        RuleId = ruleId;
    }
}

[NetSerializable, Serializable]
public sealed class ReleaseControlMsg : EuiMessageBase
{
}
