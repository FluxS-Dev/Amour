using Content.Shared._Amour.Discord;

namespace Content.Server._Amour.Discord;

public sealed class ServerDiscordLinkManager : ISharedDiscordLinkManager
{
    public bool IsLinked => true;

    public event Action<Guid>? CodeReceived;
    public event Action? StatusUpdated;
}
