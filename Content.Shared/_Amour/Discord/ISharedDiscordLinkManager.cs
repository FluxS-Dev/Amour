namespace Content.Shared._Amour.Discord;

public interface ISharedDiscordLinkManager
{
    bool IsLinked { get; }
    event Action<Guid>? CodeReceived;
    event Action? StatusUpdated;
}
