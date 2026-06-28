using Robust.Shared.Audio;

namespace Content.Server._RW.BloodCult.Runes.Summon;

[RegisterComponent]
public sealed partial class CultRuneSummonComponent : Component
{
    [DataField]
    public SoundPathSpecifier TeleportSound = new("/Audio/_RW/BloodCult/veilin.ogg");
}
