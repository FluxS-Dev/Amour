using System.Numerics;
using Content.Client._RW.UserInterface.Systems.Chat.RichText;
using Content.Shared.Chat;
using Robust.Client.ResourceManagement;
using Content.Client.UserInterface.Systems.Chat.Controls;
using Robust.Client.UserInterface;

namespace Content.Client._RW.UserInterface.Systems.Chat.Controls;

public sealed class EmojiPickerButton : ChatPopupButton<EmojiPickerPopup>
{
    [Dependency] private readonly IResourceCache _resourceCache = default!;
    [Dependency] private readonly IUserInterfaceManager _uiManager = default!;

    private const float PopupMargin = 8f;

    public event Action<string>? OnEmojiPicked;

    public EmojiPickerButton()
    {
        IoCManager.InjectDependencies(this);

        MinWidth = 34;
        ToolTip = Loc.GetString("hud-chatbox-emoji-button-tooltip");
        AddChild(ChatEmojiRichText.CreateCategoryTextureRect(_resourceCache, ChatEmojiCategory.Smileys));
        Popup.OnEmojiPicked += HandleEmojiPicked;
    }

    protected override UIBox2 GetPopupPosition()
    {
        var globalPos = GlobalPosition;
        var rootSize = _uiManager.RootControl.Size;
        var maxX = Math.Max(0, rootSize.X - EmojiPickerPopup.PopupWidth);
        var x = Math.Clamp(globalPos.X, 0, maxX);
        var y = Math.Max(0, globalPos.Y - EmojiPickerPopup.PopupHeight - PopupMargin);

        return UIBox2.FromDimensions(
            new Vector2(x, y),
            new Vector2(EmojiPickerPopup.PopupWidth, EmojiPickerPopup.PopupHeight));
    }

    public void SetAvailable(bool available)
    {
        Visible = available;
        Disabled = !available;

        if (!available && Popup.Visible)
            Popup.Close();
    }

    private void HandleEmojiPicked(string emoji)
    {
        Popup.Close();
        OnEmojiPicked?.Invoke(emoji);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
            Popup.OnEmojiPicked -= HandleEmojiPicked;
    }
}
