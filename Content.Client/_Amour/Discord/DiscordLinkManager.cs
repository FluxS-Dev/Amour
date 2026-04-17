using Content.Shared._Amour.Discord;
using Robust.Shared.Network;

namespace Content.Client._Amour.Discord;

public sealed class DiscordLinkManager : ISharedDiscordLinkManager, IPostInjectInit
{
    [Dependency] private readonly INetManager _net = default!;

    public bool IsLinked { get; private set; }

    public event Action<Guid>? CodeReceived;
    public event Action? StatusUpdated;

    private void OnCode(DiscordLinkCodeMsg message)
    {
        CodeReceived?.Invoke(message.Code);
    }

    private void OnStatus(DiscordLinkStatusMsg message)
    {
        IsLinked = message.IsLinked;
        StatusUpdated?.Invoke();
    }

    void IPostInjectInit.PostInject()
    {
        _net.RegisterNetMessage<DiscordLinkCodeMsg>(OnCode);
        _net.RegisterNetMessage<DiscordLinkRequestMsg>();
        _net.RegisterNetMessage<DiscordLinkStatusMsg>(OnStatus);
    }
}
