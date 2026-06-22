using Content.Client.Eui;
using Robust.Client.Graphics;

namespace Content.Client._RW.Zombies.UI;

public sealed class ZombieRoleEui : BaseEui
{
    private readonly ZombieRoleMenu _menu = new();

    public override void Opened()
    {
        IoCManager.Resolve<IClyde>().RequestWindowAttention();
        _menu.OpenCentered();
    }

    public override void Closed()
    {
        base.Closed();

        _menu.Close();
    }
}
