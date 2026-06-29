using Content.Client.Eui;
using Content.Shared.Eui;
using Content.Shared._RW.GameFlowControl;
using JetBrains.Annotations;

namespace Content.Client._RW.GameFlowControl;

[UsedImplicitly]
public sealed class GameFlowControlEui : BaseEui
{
    private readonly GameFlowControlWindow _window;

    public GameFlowControlEui()
    {
        _window = new GameFlowControlWindow(this);
        _window.OnClose += SendClosedMessage;
    }

    public override void Opened()
    {
        base.Opened();
        _window.OpenCentered();
    }

    public override void Closed()
    {
        base.Closed();
        _window.OnClose -= SendClosedMessage;
        _window.Close();
        _window.Dispose();
    }

    private void SendClosedMessage()
    {
        SendMessage(new CloseEuiMessage());
    }

    public override void HandleState(EuiStateBase state)
    {
        base.HandleState(state);

        if (state is not GameFlowControlEuiState s)
            return;

        _window.UpdateState(s);
    }

    public new void SendMessage(EuiMessageBase message)
    {
        base.SendMessage(message);
    }
}
