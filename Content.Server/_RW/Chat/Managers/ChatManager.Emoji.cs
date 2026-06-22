using Content.Shared.Chat;

namespace Content.Server.Chat.Managers;

internal sealed partial class ChatManager
{
    private ChatSelectChannel _emojiAllowedChannels = ChatEmoji.DefaultAllowedChannels;

    private void OnEmojiAllowedChannelsChanged(string raw)
    {
        _emojiAllowedChannels = ChatEmoji.ParseAllowedChannels(raw);
    }

    private void ApplyEmojiPolicy(ChatChannel channel, ref string message, ref string wrappedMessage)
    {
        message = ChatEmoji.ApplyPolicy(message, channel, _emojiAllowedChannels);
        wrappedMessage = ChatEmoji.ApplyPolicy(wrappedMessage, channel, _emojiAllowedChannels);
    }
}
