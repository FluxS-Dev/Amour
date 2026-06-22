using Content.Client._RW.UserInterface.Systems.Chat.Controls;

namespace Content.Client.UserInterface.Systems.Chat.Controls;

public partial class ChatInputBox
{
    public EmojiPickerButton EmojiButton { get; private set; } = default!;

    private void InitializeEmojiPicker()
    {
        EmojiButton = new EmojiPickerButton
        {
            Name = "EmojiButton",
            StyleClasses = { "chatFilterOptionButton" }
        };
        EmojiButton.OnEmojiPicked += InsertEmoji;
        Container.AddChild(EmojiButton);
    }

    private void InsertEmoji(string emoji)
    {
        Input.InsertAtCursor(emoji);
        Input.GrabKeyboardFocus();
    }

    public void SetEmojiAllowed(bool allowed)
    {
        EmojiButton.SetAvailable(allowed);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            EmojiButton.OnEmojiPicked -= InsertEmoji;
            ChannelSelector.OnChannelSelect -= UpdateActiveChannel;
        }

        base.Dispose(disposing);
    }
}
