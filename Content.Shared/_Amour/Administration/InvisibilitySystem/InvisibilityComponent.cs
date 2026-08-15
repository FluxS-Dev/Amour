using Content.Shared.Actions;
using Robust.Shared.Prototypes;

namespace Content.Shared._Amour.Administration;

[RegisterComponent]
[Access(typeof(SharedInvisibilitySystem))]
public sealed partial class InvisibilityComponent : Component
{
    [ViewVariables]
    public bool Invisible;

    public float? DefaultAlpha;
}
