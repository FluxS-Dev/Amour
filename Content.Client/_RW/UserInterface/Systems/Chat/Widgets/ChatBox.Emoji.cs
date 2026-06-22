using Content.Client._RW.UserInterface.Systems.Chat.RichText;
using Content.Shared.Chat;
using Robust.Shared.Prototypes;

namespace Content.Client.UserInterface.Systems.Chat.Widgets;

public partial class ChatBox
{
    [Dependency] private readonly IPrototypeManager _emojiPrototypeManager = default!;

    private bool _suppressEmojiAliasRewrite;

    private string PrepareEmojiMarkup(string markup, bool allowAliases)
    {
        return ChatEmojiRichText.ReplaceEmojiMarkup(markup, allowAliases, _emojiPrototypeManager);
    }

    private bool RewriteEmojiAliases()
    {
        if (_suppressEmojiAliasRewrite ||
            !_controller.IsEmojiAllowed(_controller.ResolveEffectiveInputChannel(this)))
        {
            return false;
        }

        var input = ChatInput.Input;
        var rewritten = ChatEmoji.ReplaceAliases(
            input.Text,
            input.CursorPosition,
            _emojiPrototypeManager,
            out var newCursorPosition);

        if (string.Equals(rewritten, input.Text, StringComparison.Ordinal))
            return false;

        _suppressEmojiAliasRewrite = true;
        try
        {
            input.SetText(rewritten);
            input.CursorPosition = newCursorPosition;
            input.SelectionStart = newCursorPosition;
        }
        finally
        {
            _suppressEmojiAliasRewrite = false;
        }

        return true;
    }
}
