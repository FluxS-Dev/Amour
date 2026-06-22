using System.Linq;
using System.Numerics;
using System.Text;
using Content.Shared.Chat;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client._RW.UserInterface.Systems.Chat.RichText;

public static class ChatEmojiRichText
{
    private readonly record struct EmojiMatchPattern(string Value, ChatEmojiDefinition Emoji);

    public const string EmojiMarkupTag = "chatemoji";

    private static readonly Dictionary<char, EmojiMatchPattern[]> DefinitionsByFirstChar =
        BuildDefinitionsByFirstChar();

    public static string ReplaceEmojiMarkup(
        string markup,
        bool allowAliasMarkup = true,
        IPrototypeManager? prototypeManager = null)
    {
        var builder = new StringBuilder(markup.Length + 32);
        var plainStart = 0;
        var index = 0;

        while (index < markup.Length)
        {
            if (markup[index] == '\\' && index + 1 < markup.Length)
            {
                index += 2;
                continue;
            }

            if (markup[index] == '[')
            {
                AppendRawTextWithEmojiMarkup(
                    builder,
                    markup,
                    plainStart,
                    index,
                    allowAliasMarkup,
                    prototypeManager);

                var tagEnd = markup.IndexOf(']', index + 1);
                if (tagEnd == -1)
                {
                    AppendRawTextWithEmojiMarkup(
                        builder,
                        markup,
                        index,
                        markup.Length,
                        allowAliasMarkup,
                        prototypeManager);
                    return builder.ToString();
                }

                builder.Append(markup, index, tagEnd - index + 1);
                index = tagEnd + 1;
                plainStart = index;
                continue;
            }

            index += char.IsSurrogatePair(markup, index) ? 2 : 1;
        }

        if (plainStart < markup.Length)
        {
            AppendRawTextWithEmojiMarkup(
                builder,
                markup,
                plainStart,
                markup.Length,
                allowAliasMarkup,
                prototypeManager);
        }

        return builder.ToString();
    }

    public static TextureRect CreateInlineTextureRect(
        IResourceCache resourceCache,
        ChatEmojiDefinition emoji)
    {
        return CreateTextureRect(resourceCache, emoji, 5f / 18f, 25f, new Thickness(1f, 2f));
    }

    public static TextureRect CreatePickerTextureRect(
        IResourceCache resourceCache,
        ChatEmojiDefinition emoji)
    {
        return CreateTextureRect(resourceCache, emoji, 1f / 3f, 24f, new Thickness(3f));
    }

    public static TextureRect CreateCategoryTextureRect(
        IResourceCache resourceCache,
        ChatEmojiCategory category)
    {
        return CreateCategoryTextureRect(resourceCache, ChatEmoji.GetCategoryIcon(category));
    }

    public static TextureRect CreateCategoryTextureRect(
        IResourceCache resourceCache,
        ChatEmojiDefinition emoji)
    {
        return CreateTextureRect(resourceCache, emoji, 11f / 36f, 22f, new Thickness(1f));
    }

    public static FormattedMessage BuildPreviewMessage(ChatEmojiDefinition emoji)
    {
        var emojiMarkup = $"[{EmojiMarkupTag} alias=\"{emoji.Alias}\"/]";
        return FormattedMessage.FromMarkupOrThrow(
            Loc.GetString(
                "hud-chatbox-emoji-preview",
                ("emoji", emojiMarkup),
                ("alias", FormattedMessage.EscapeText(emoji.Alias))));
    }

    private static TextureRect CreateTextureRect(
        IResourceCache resourceCache,
        ChatEmojiDefinition emoji,
        float textureScale,
        float minSize,
        Thickness margin)
    {
        return new TextureRect
        {
            Texture = ResolveTexture(resourceCache, emoji),
            TextureScale = new Vector2(textureScale),
            Stretch = TextureRect.StretchMode.KeepCentered,
            HorizontalAlignment = Control.HAlignment.Center,
            VerticalAlignment = Control.VAlignment.Center,
            CanShrink = true,
            MinSize = new Vector2(minSize),
            Margin = margin
        };
    }

    private static Texture ResolveTexture(
        IResourceCache resourceCache,
        ChatEmojiDefinition emoji)
    {
        if (resourceCache.TryGetResource<RSIResource>(emoji.TexturePath, out var emojiResource) &&
            emojiResource?.RSI != null &&
            emojiResource.RSI.TryGetState(emoji.TextureState, out var state))
        {
            return state.Frame0;
        }

        return resourceCache.GetFallback<TextureResource>().Texture;
    }

    private static void AppendRawTextWithEmojiMarkup(
        StringBuilder builder,
        string text,
        int start,
        int end,
        bool allowAliasMarkup,
        IPrototypeManager? prototypeManager)
    {
        var plainStart = start;
        var index = start;

        while (index < end)
        {
            if (text[index] == '\\' && index + 1 < end)
            {
                index += 2;
                continue;
            }

            if (allowAliasMarkup &&
                ChatEmoji.TryMatchAlias(
                    text,
                    index,
                    end,
                    prototypeManager,
                    out var aliasEmoji,
                    out var aliasLength))
            {
                builder.Append(text, plainStart, index - plainStart);
                AppendEmojiMarkup(builder, aliasEmoji);
                index += aliasLength;
                plainStart = index;
                continue;
            }

            if (TryMatchEmoji(text, index, end, out var emoji, out var emojiLength))
            {
                builder.Append(text, plainStart, index - plainStart);
                AppendEmojiMarkup(builder, emoji);
                index += emojiLength;
                plainStart = index;
                continue;
            }

            index += char.IsSurrogatePair(text, index) ? 2 : 1;
        }

        if (plainStart < end)
            builder.Append(text, plainStart, end - plainStart);
    }

    private static void AppendEmojiMarkup(StringBuilder builder, ChatEmojiDefinition emoji)
    {
        builder.Append('[')
            .Append(EmojiMarkupTag)
            .Append(" alias=\"")
            .Append(emoji.Alias)
            .Append("\"/]");
    }

    private static bool TryMatchEmoji(
        string text,
        int index,
        int endExclusive,
        out ChatEmojiDefinition emoji,
        out int consumedLength)
    {
        emoji = default;
        consumedLength = 0;

        if (index >= text.Length ||
            !DefinitionsByFirstChar.TryGetValue(text[index], out var definitions))
        {
            return false;
        }

        foreach (var definition in definitions)
        {
            if (definition.Value.Length > endExclusive - index ||
                string.CompareOrdinal(text, index, definition.Value, 0, definition.Value.Length) != 0)
            {
                continue;
            }

            emoji = definition.Emoji;
            consumedLength = definition.Value.Length;
            return true;
        }

        return false;
    }

    private static Dictionary<char, EmojiMatchPattern[]> BuildDefinitionsByFirstChar()
    {
        var grouped = new Dictionary<char, List<EmojiMatchPattern>>();

        foreach (var definition in ChatEmoji.All)
        {
            AddMatchPattern(grouped, definition.Value, definition);

            var simplifiedValue = StripVariationSelectors(definition.Value);
            if (!string.Equals(simplifiedValue, definition.Value, StringComparison.Ordinal))
                AddMatchPattern(grouped, simplifiedValue, definition);
        }

        return grouped.ToDictionary(
            pair => pair.Key,
            pair => pair.Value
                .GroupBy(pattern => pattern.Value)
                .Select(group => group.First())
                .OrderByDescending(pattern => pattern.Value.Length)
                .ThenBy(pattern => pattern.Emoji.Alias)
                .ToArray());
    }

    private static void AddMatchPattern(
        Dictionary<char, List<EmojiMatchPattern>> grouped,
        string value,
        ChatEmojiDefinition definition)
    {
        if (string.IsNullOrEmpty(value))
            return;

        if (!grouped.TryGetValue(value[0], out var list))
        {
            list = new List<EmojiMatchPattern>();
            grouped[value[0]] = list;
        }

        list.Add(new EmojiMatchPattern(value, definition));
    }

    private static string StripVariationSelectors(string value)
    {
        var builder = new StringBuilder(value.Length);
        foreach (var rune in value.EnumerateRunes())
        {
            if (rune.Value is not (>= 0xFE00 and <= 0xFE0F) and
                not (>= 0xE0100 and <= 0xE01EF))
            {
                builder.Append(rune);
            }
        }

        return builder.ToString();
    }
}
