using Content.Shared.Chat;
using NUnit.Framework;

namespace Content.Tests.Shared._RW.Chat;

[TestFixture]
[TestOf(typeof(ChatEmoji))]
public sealed class ChatEmojiTest
{
    [Test]
    public void ParseAllowedChannels()
    {
        var expected =
            ChatSelectChannel.Local |
            ChatSelectChannel.Radio |
            ChatSelectChannel.OOC;

        Assert.That(ChatEmoji.ParseAllowedChannels("Local, radio | OOC"), Is.EqualTo(expected));
        Assert.That(ChatEmoji.ParseAllowedChannels("all"), Is.EqualTo(ChatEmoji.AllAllowedChannels));
        Assert.That(ChatEmoji.ParseAllowedChannels("none"), Is.EqualTo(ChatSelectChannel.None));
        Assert.That(ChatEmoji.ParseAllowedChannels("invalid"), Is.EqualTo(ChatEmoji.DefaultAllowedChannels));
    }

    [Test]
    public void QuietEmotesUseEmotePolicy()
    {
        Assert.That(
            ChatEmoji.IsAllowed(ChatSelectChannel.Emotes, ChatSelectChannel.QuietEmotes),
            Is.True);
    }

    [Test]
    public void ReplaceAliasesPreservesCursorPosition()
    {
        Assert.That(ChatEmoji.TryGet("smile", out var smile), Is.True);

        const string input = "before :smile: after";
        var result = ChatEmoji.ReplaceAliases(
            input,
            input.Length,
            null,
            out var cursorPosition);
        var expected = $"before {smile.Value} after";

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expected));
            Assert.That(cursorPosition, Is.EqualTo(expected.Length));
        });
    }

    [Test]
    public void StripDirectEmojiPreservesText()
    {
        Assert.That(ChatEmoji.TryGet("smile", out var smile), Is.True);

        var input = $"before {smile.Value} after";
        Assert.That(ChatEmoji.StripDirectEmoji(input), Is.EqualTo("before  after"));
    }
}
