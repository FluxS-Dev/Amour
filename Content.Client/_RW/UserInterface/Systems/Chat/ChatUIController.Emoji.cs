using Content.Client.UserInterface.Systems.Chat.Widgets;
using Content.Shared.Chat;

namespace Content.Client.UserInterface.Systems.Chat;

public sealed partial class ChatUIController
{
    private ChatSelectChannel _emojiAllowedChannels = ChatEmoji.DefaultAllowedChannels;

    public ChatSelectChannel ResolveEffectiveInputChannel(ChatBox box)
    {
        var prefixChannel = SplitInputContents(box.ChatInput.Input.Text.ToLowerInvariant()).chatChannel;
        var selectedChannel = box.SelectedChannel == ChatSelectChannel.None
            ? GetPreferredChannel()
            : box.SelectedChannel;

        return prefixChannel != ChatSelectChannel.None
            ? prefixChannel
            : MapLocalIfGhost(selectedChannel);
    }

    public bool IsEmojiAllowed(ChatSelectChannel channel)
    {
        return ChatEmoji.IsAllowed(_emojiAllowedChannels, MapLocalIfGhost(channel));
    }

    public bool IsEmojiAllowed(ChatChannel channel)
    {
        return ChatEmoji.IsAllowed(_emojiAllowedChannels, channel);
    }

    private void UpdateEmojiAvailability(ChatBox box)
    {
        box.ChatInput.SetEmojiAllowed(IsEmojiAllowed(ResolveEffectiveInputChannel(box)));
    }

    private void OnEmojiAllowedChannelsChanged(string raw)
    {
        _emojiAllowedChannels = ChatEmoji.ParseAllowedChannels(raw);

        foreach (var chat in _chats)
        {
            UpdateSelectedChannel(chat);
        }
    }
}
