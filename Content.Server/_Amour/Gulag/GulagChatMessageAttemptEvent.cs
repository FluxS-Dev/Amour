using Robust.Shared.Player;

namespace Content.Server._Amour.Gulag;

public sealed class GulagChatMessageAttemptEvent(ICommonSession player, string message) : CancellableEntityEventArgs
{
    public readonly ICommonSession Player = player;

    public readonly string Message = message;
}
