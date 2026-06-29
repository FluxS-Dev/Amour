using Content.Shared._RW.GameFlowControl;

namespace Content.Client._RW.GameFlowControl;

public sealed class GameFlowControlSystem : SharedGameFlowControlSystem
{
    public string? Occupier { get; private set; }

    public event Action? OnStateChanged;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeNetworkEvent<GameFlowControlStateEvent>(OnStateReceived);

        // Request initial state on system startup
        RaiseNetworkEvent(new RequestGameFlowControlStateEvent());
    }

    private void OnStateReceived(GameFlowControlStateEvent ev, EntitySessionEventArgs args)
    {
        Occupier = ev.OccupierName;
        OnStateChanged?.Invoke();
    }
}
