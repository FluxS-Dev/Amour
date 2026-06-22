using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Content.Shared.Chat.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared.Chat;

public enum ChatEmojiCategory : byte
{
    Custom,
    Smileys,
    Nature,
    Food,
    Activities,
    Travel,
    Objects,
    Symbols,
    Flags
}

public readonly record struct ChatEmojiDefinition(
    string Alias,
    string Value,
    ChatEmojiCategory Category,
    ResPath? RsiPath = null,
    string? RsiState = null)
{
    public bool HasDirectValue => !string.IsNullOrEmpty(Value);
    public string InsertText => HasDirectValue ? Value : $":{Alias}:";
    public ResPath TexturePath => RsiPath ?? ChatEmoji.DefaultEmojiRsiPath;
    public string TextureState => string.IsNullOrWhiteSpace(RsiState) ? Alias : RsiState;
}

public static class ChatEmoji
{
    public const string DefaultAllowedChannelsCVar = "LOOC,OOC,Dead,Admin";

    public const ChatSelectChannel DefaultAllowedChannels =
        ChatSelectChannel.LOOC |
        ChatSelectChannel.OOC |
        ChatSelectChannel.Dead |
        ChatSelectChannel.Admin;

    public const ChatSelectChannel AllAllowedChannels =
        ChatSelectChannel.Local |
        ChatSelectChannel.Whisper |
        ChatSelectChannel.Radio |
        ChatSelectChannel.LOOC |
        ChatSelectChannel.OOC |
        ChatSelectChannel.Emotes |
        ChatSelectChannel.Dead |
        ChatSelectChannel.Admin |
        ChatSelectChannel.Telepathic |
        ChatSelectChannel.CollectiveMind;

    public static readonly ResPath DefaultEmojiRsiPath = new("/Textures/_RW/Interface/Chat/emoji.rsi");

    private static readonly ChatEmojiCategory[] BuiltInCategoryOrder =
    {
        ChatEmojiCategory.Smileys,
        ChatEmojiCategory.Nature,
        ChatEmojiCategory.Food,
        ChatEmojiCategory.Activities,
        ChatEmojiCategory.Travel,
        ChatEmojiCategory.Objects,
        ChatEmojiCategory.Symbols,
        ChatEmojiCategory.Flags
    };

    private static readonly Regex AliasRegex =
        new(":([a-z0-9_+-]+):", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private static readonly ChatEmojiDefinition[] Definitions =
    {
        new("grinning", "\ud83d\ude00", ChatEmojiCategory.Smileys),
        new("grin", "\ud83d\ude01", ChatEmojiCategory.Smileys),
        new("joy", "\ud83d\ude02", ChatEmojiCategory.Smileys),
        new("smiley", "\ud83d\ude03", ChatEmojiCategory.Smileys),
        new("smile", "\ud83d\ude04", ChatEmojiCategory.Smileys),
        new("sweat_smile", "\ud83d\ude05", ChatEmojiCategory.Smileys),
        new("laughing", "\ud83d\ude06", ChatEmojiCategory.Smileys),
        new("innocent", "\ud83d\ude07", ChatEmojiCategory.Smileys),
        new("wink", "\ud83d\ude09", ChatEmojiCategory.Smileys),
        new("blush", "\ud83d\ude0a", ChatEmojiCategory.Smileys),
        new("heart_eyes", "\ud83d\ude0d", ChatEmojiCategory.Smileys),
        new("sunglasses", "\ud83d\ude0e", ChatEmojiCategory.Smileys),
        new("smirk", "\ud83d\ude0f", ChatEmojiCategory.Smileys),
        new("neutral_face", "\ud83d\ude10", ChatEmojiCategory.Smileys),
        new("expressionless", "\ud83d\ude11", ChatEmojiCategory.Smileys),
        new("unamused", "\ud83d\ude12", ChatEmojiCategory.Smileys),
        new("sweat", "\ud83d\ude13", ChatEmojiCategory.Smileys),
        new("pensive", "\ud83d\ude14", ChatEmojiCategory.Smileys),
        new("worried", "\ud83d\ude1f", ChatEmojiCategory.Smileys),
        new("pouting_face", "\ud83d\ude20", ChatEmojiCategory.Smileys),
        new("rage", "\ud83d\ude21", ChatEmojiCategory.Smileys),
        new("cry", "\ud83d\ude22", ChatEmojiCategory.Smileys),
        new("sob", "\ud83d\ude2d", ChatEmojiCategory.Smileys),
        new("scream", "\ud83d\ude31", ChatEmojiCategory.Smileys),
        new("sleeping", "\ud83d\ude34", ChatEmojiCategory.Smileys),
        new("mask", "\ud83d\ude37", ChatEmojiCategory.Smileys),
        new("slightly_smiling_face", "\ud83d\ude42", ChatEmojiCategory.Smileys),
        new("upside_down_face", "\ud83d\ude43", ChatEmojiCategory.Smileys),
        new("rolling_eyes", "\ud83d\ude44", ChatEmojiCategory.Smileys),
        new("thinking", "\ud83e\udd14", ChatEmojiCategory.Smileys),
        new("rofl", "\ud83e\udd23", ChatEmojiCategory.Smileys),
        new("star_struck", "\ud83e\udd29", ChatEmojiCategory.Smileys),
        new("zany_face", "\ud83e\udd2a", ChatEmojiCategory.Smileys),
        new("pleading_face", "\ud83e\udd7a", ChatEmojiCategory.Smileys),
        new("smiling_face_with_tear", "\ud83e\udd72", ChatEmojiCategory.Smileys),
        new("partying_face", "\ud83e\udd73", ChatEmojiCategory.Smileys),
        new("saluting_face", "\ud83e\udee1", ChatEmojiCategory.Smileys),
        new("wave", "\ud83d\udc4b", ChatEmojiCategory.Smileys),
        new("thumbsup", "\ud83d\udc4d", ChatEmojiCategory.Smileys),
        new("thumbsdown", "\ud83d\udc4e", ChatEmojiCategory.Smileys),
        new("clap", "\ud83d\udc4f", ChatEmojiCategory.Smileys),
        new("point_left", "\ud83d\udc48", ChatEmojiCategory.Smileys),
        new("point_right", "\ud83d\udc49", ChatEmojiCategory.Smileys),
        new("ok_hand", "\ud83d\udc4c", ChatEmojiCategory.Smileys),
        new("fist", "\ud83d\udc4a", ChatEmojiCategory.Smileys),
        new("muscle", "\ud83d\udcaa", ChatEmojiCategory.Smileys),
        new("pray", "\ud83d\ude4f", ChatEmojiCategory.Smileys),
        new("handshake", "\ud83e\udd1d", ChatEmojiCategory.Smileys),
        new("pinched_fingers", "\ud83e\udd0c", ChatEmojiCategory.Smileys),
        new("v", "✌\ufe0f", ChatEmojiCategory.Smileys),
        new("writing_hand", "✍\ufe0f", ChatEmojiCategory.Smileys),
        new("point_up", "☝\ufe0f", ChatEmojiCategory.Smileys),
        new("dog", "\ud83d\udc36", ChatEmojiCategory.Nature),
        new("cat", "\ud83d\udc31", ChatEmojiCategory.Nature),
        new("mouse", "\ud83d\udc2d", ChatEmojiCategory.Nature),
        new("wolf", "\ud83d\udc3a", ChatEmojiCategory.Nature),
        new("fox", "\ud83e\udd8a", ChatEmojiCategory.Nature),
        new("herb", "\ud83c\udf3f", ChatEmojiCategory.Nature),
        new("four_leaf_clover", "\ud83c\udf40", ChatEmojiCategory.Nature),
        new("rose", "\ud83c\udf39", ChatEmojiCategory.Nature),
        new("skull", "\ud83d\udc80", ChatEmojiCategory.Nature),
        new("ghost", "\ud83d\udc7b", ChatEmojiCategory.Nature),
        new("alien", "\ud83d\udc7d", ChatEmojiCategory.Nature),
        new("robot", "\ud83e\udd16", ChatEmojiCategory.Nature),
        new("fire", "\ud83d\udd25", ChatEmojiCategory.Nature),
        new("sparkles", "✨", ChatEmojiCategory.Nature),
        new("star", "⭐", ChatEmojiCategory.Nature),
        new("sunny", "☀\ufe0f", ChatEmojiCategory.Nature),
        new("moon", "\ud83c\udf19", ChatEmojiCategory.Nature),
        new("zap", "⚡", ChatEmojiCategory.Nature),
        new("snowflake", "❄\ufe0f", ChatEmojiCategory.Nature),
        new("cloud_with_rain", "\ud83c\udf27\ufe0f", ChatEmojiCategory.Nature),
        new("apple", "\ud83c\udf4e", ChatEmojiCategory.Food),
        new("burger", "\ud83c\udf54", ChatEmojiCategory.Food),
        new("pizza", "\ud83c\udf55", ChatEmojiCategory.Food),
        new("fries", "\ud83c\udf5f", ChatEmojiCategory.Food),
        new("taco", "\ud83c\udf2e", ChatEmojiCategory.Food),
        new("coffee", "☕", ChatEmojiCategory.Food),
        new("tea", "\ud83c\udf75", ChatEmojiCategory.Food),
        new("beer", "\ud83c\udf7a", ChatEmojiCategory.Food),
        new("tropical_drink", "\ud83c\udf79", ChatEmojiCategory.Food),
        new("birthday", "\ud83c\udf82", ChatEmojiCategory.Food),
        new("icecream", "\ud83c\udf66", ChatEmojiCategory.Food),
        new("lollipop", "\ud83c\udf6d", ChatEmojiCategory.Food),
        new("bowl_with_spoon", "\ud83e\udd63", ChatEmojiCategory.Food),
        new("soccer", "⚽", ChatEmojiCategory.Activities),
        new("basketball", "\ud83c\udfc0", ChatEmojiCategory.Activities),
        new("football", "\ud83c\udfc8", ChatEmojiCategory.Activities),
        new("baseball", "⚾", ChatEmojiCategory.Activities),
        new("video_game", "\ud83c\udfae", ChatEmojiCategory.Activities),
        new("game_die", "\ud83c\udfb2", ChatEmojiCategory.Activities),
        new("dart", "\ud83c\udfaf", ChatEmojiCategory.Activities),
        new("trophy", "\ud83c\udfc6", ChatEmojiCategory.Activities),
        new("medal", "\ud83c\udfc5", ChatEmojiCategory.Activities),
        new("musical_note", "\ud83c\udfb5", ChatEmojiCategory.Activities),
        new("microphone", "\ud83c\udfa4", ChatEmojiCategory.Activities),
        new("art", "\ud83c\udfa8", ChatEmojiCategory.Activities),
        new("performing_arts", "\ud83c\udfad", ChatEmojiCategory.Activities),
        new("car", "\ud83d\ude97", ChatEmojiCategory.Travel),
        new("taxi", "\ud83d\ude95", ChatEmojiCategory.Travel),
        new("bus", "\ud83d\ude8c", ChatEmojiCategory.Travel),
        new("train", "\ud83d\ude86", ChatEmojiCategory.Travel),
        new("airplane", "✈\ufe0f", ChatEmojiCategory.Travel),
        new("rocket", "\ud83d\ude80", ChatEmojiCategory.Travel),
        new("ship", "\ud83d\udea2", ChatEmojiCategory.Travel),
        new("satellite", "\ud83d\udef0\ufe0f", ChatEmojiCategory.Travel),
        new("bicycle", "\ud83d\udeb2", ChatEmojiCategory.Travel),
        new("motorcycle", "\ud83c\udfcd\ufe0f", ChatEmojiCategory.Travel),
        new("package", "\ud83d\udce6", ChatEmojiCategory.Objects),
        new("gift", "\ud83c\udf81", ChatEmojiCategory.Objects),
        new("bulb", "\ud83d\udca1", ChatEmojiCategory.Objects),
        new("wrench", "\ud83d\udd27", ChatEmojiCategory.Objects),
        new("gear", "⚙\ufe0f", ChatEmojiCategory.Objects),
        new("hammer", "\ud83d\udd28", ChatEmojiCategory.Objects),
        new("hammer_and_wrench", "\ud83d\udee0\ufe0f", ChatEmojiCategory.Objects),
        new("computer", "\ud83d\udcbb", ChatEmojiCategory.Objects),
        new("lock", "\ud83d\udd12", ChatEmojiCategory.Objects),
        new("key", "\ud83d\udd11", ChatEmojiCategory.Objects),
        new("moneybag", "\ud83d\udcb0", ChatEmojiCategory.Objects),
        new("gem", "\ud83d\udc8e", ChatEmojiCategory.Objects),
        new("book", "\ud83d\udcd6", ChatEmojiCategory.Objects),
        new("pill", "\ud83d\udc8a", ChatEmojiCategory.Objects),
        new("heart", "❤\ufe0f", ChatEmojiCategory.Symbols),
        new("orange_heart", "\ud83e\udde1", ChatEmojiCategory.Symbols),
        new("yellow_heart", "\ud83d\udc9b", ChatEmojiCategory.Symbols),
        new("green_heart", "\ud83d\udc9a", ChatEmojiCategory.Symbols),
        new("blue_heart", "\ud83d\udc99", ChatEmojiCategory.Symbols),
        new("purple_heart", "\ud83d\udc9c", ChatEmojiCategory.Symbols),
        new("broken_heart", "\ud83d\udc94", ChatEmojiCategory.Symbols),
        new("100", "\ud83d\udcaf", ChatEmojiCategory.Symbols),
        new("boom", "\ud83d\udca5", ChatEmojiCategory.Symbols),
        new("warning", "⚠\ufe0f", ChatEmojiCategory.Symbols),
        new("white_check_mark", "✅", ChatEmojiCategory.Symbols),
        new("x", "❌", ChatEmojiCategory.Symbols),
        new("question", "❓", ChatEmojiCategory.Symbols),
        new("grey_question", "❔", ChatEmojiCategory.Symbols),
        new("exclamation", "❗", ChatEmojiCategory.Symbols),
        new("grey_exclamation", "❕", ChatEmojiCategory.Symbols),
        new("recycle", "♻\ufe0f", ChatEmojiCategory.Symbols),
        new("peace", "☮\ufe0f", ChatEmojiCategory.Symbols),
        new("biohazard", "☣\ufe0f", ChatEmojiCategory.Symbols),
        new("radioactive", "☢\ufe0f", ChatEmojiCategory.Symbols),
        new("infinity", "♾\ufe0f", ChatEmojiCategory.Symbols),
        new("zzz", "\ud83d\udca4", ChatEmojiCategory.Symbols),
        new("triangular_flag_on_post", "\ud83d\udea9", ChatEmojiCategory.Flags),
        new("checkered_flag", "\ud83c\udfc1", ChatEmojiCategory.Flags),
        new("black_flag", "\ud83c\udff4", ChatEmojiCategory.Flags),
        new("white_flag", "\ud83c\udff3\ufe0f", ChatEmojiCategory.Flags),
        new("rainbow_flag", "\ud83c\udff3\ufe0f\u200d\ud83c\udf08", ChatEmojiCategory.Flags),
        new("pirate_flag", "\ud83c\udff4\u200d☠\ufe0f", ChatEmojiCategory.Flags),
        new("flag_ru", "\ud83c\uddf7\ud83c\uddfa", ChatEmojiCategory.Flags),
        new("flag_us", "\ud83c\uddfa\ud83c\uddf8", ChatEmojiCategory.Flags),
        new("flag_gb", "\ud83c\uddec\ud83c\udde7", ChatEmojiCategory.Flags),
        new("flag_fr", "\ud83c\uddeb\ud83c\uddf7", ChatEmojiCategory.Flags),
        new("flag_de", "\ud83c\udde9\ud83c\uddea", ChatEmojiCategory.Flags),
        new("flag_ua", "\ud83c\uddfa\ud83c\udde6", ChatEmojiCategory.Flags)
    };

    private static readonly Dictionary<string, ChatEmojiDefinition> AliasMap = BuildAliasMap();

    private static readonly HashSet<int> KnownEmojiRunes = BuildKnownEmojiRuneSet();

    public static IReadOnlyList<ChatEmojiDefinition> All => Definitions;

    public static IEnumerable<ChatEmojiCategory> GetCategoryOrder(IPrototypeManager? prototypeManager = null)
    {
        if (HasCustomEmojis(prototypeManager))
            yield return ChatEmojiCategory.Custom;

        foreach (var category in BuiltInCategoryOrder)
            yield return category;
    }

    public static bool HasCustomEmojis(IPrototypeManager? prototypeManager)
    {
        return prototypeManager != null &&
               prototypeManager.EnumeratePrototypes<ChatCustomEmojiPrototype>().Any();
    }

    public static IEnumerable<ChatEmojiDefinition> EnumerateCategory(
        ChatEmojiCategory category,
        IPrototypeManager? prototypeManager = null)
    {
        if (category == ChatEmojiCategory.Custom)
        {
            if (prototypeManager == null)
                yield break;

            foreach (var definition in EnumerateCustomDefinitions(prototypeManager))
                yield return definition;

            yield break;
        }

        foreach (var definition in Definitions)
        {
            if (definition.Category == category)
                yield return definition;
        }
    }

    public static IEnumerable<ChatEmojiDefinition> EnumerateAll(IPrototypeManager? prototypeManager = null)
    {
        foreach (var definition in Definitions)
            yield return definition;

        if (prototypeManager == null)
            yield break;

        foreach (var definition in EnumerateCustomDefinitions(prototypeManager))
            yield return definition;
    }

    public static bool TryGet(string alias, out ChatEmojiDefinition definition)
    {
        return AliasMap.TryGetValue(alias.ToLowerInvariant(), out definition);
    }

    public static bool TryGet(
        string alias,
        IPrototypeManager? prototypeManager,
        out ChatEmojiDefinition definition)
    {
        if (TryGet(alias, out definition))
            return true;

        if (prototypeManager != null)
        {
            foreach (var customEmoji in prototypeManager.EnumeratePrototypes<ChatCustomEmojiPrototype>())
            {
                if (!IsValidAlias(customEmoji.ID.AsSpan()) ||
                    !string.Equals(customEmoji.ID, alias, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                definition = CreateCustomDefinition(customEmoji);
                return true;
            }
        }

        definition = default;
        return false;
    }

    public static ChatEmojiDefinition GetCategoryIcon(ChatEmojiCategory category)
    {
        return TryGet(GetCategoryIconAlias(category), out var definition)
            ? definition
            : Definitions[0];
    }

    private static string GetCategoryIconAlias(ChatEmojiCategory category)
    {
        return category switch
        {
            ChatEmojiCategory.Nature => "herb",
            ChatEmojiCategory.Food => "coffee",
            ChatEmojiCategory.Activities => "video_game",
            ChatEmojiCategory.Travel => "bicycle",
            ChatEmojiCategory.Objects => "hammer_and_wrench",
            ChatEmojiCategory.Symbols => "heart",
            ChatEmojiCategory.Flags => "triangular_flag_on_post",
            _ => "grinning"
        };
    }

    public static ChatSelectChannel ParseAllowedChannels(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return DefaultAllowedChannels;

        var trimmed = raw.Trim();
        if (string.Equals(trimmed, "all", StringComparison.OrdinalIgnoreCase) || trimmed == "*")
            return AllAllowedChannels;

        if (string.Equals(trimmed, "none", StringComparison.OrdinalIgnoreCase))
            return ChatSelectChannel.None;

        var resolved = ChatSelectChannel.None;
        var tokens = trimmed.Split(
            new[] { ' ', ',', ';', '|', '\t', '\r', '\n' },
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var token in tokens)
        {
            if (string.Equals(token, "all", StringComparison.OrdinalIgnoreCase) || token == "*")
                return AllAllowedChannels;

            if (string.Equals(token, "none", StringComparison.OrdinalIgnoreCase))
                return ChatSelectChannel.None;

            if (Enum.TryParse<ChatSelectChannel>(token, true, out var parsed))
                resolved |= parsed & AllAllowedChannels;
        }

        return resolved == ChatSelectChannel.None ? DefaultAllowedChannels : resolved;
    }

    public static bool IsAllowed(ChatSelectChannel allowedChannels, ChatSelectChannel channel)
    {
        if (channel == ChatSelectChannel.QuietEmotes)
            channel = ChatSelectChannel.Emotes;

        return channel != ChatSelectChannel.None &&
               channel != ChatSelectChannel.Console &&
               (allowedChannels & channel) != 0;
    }

    public static bool IsAllowed(ChatSelectChannel allowedChannels, ChatChannel channel)
    {
        return TryMapChannel(channel, out var selectChannel) &&
               IsAllowed(allowedChannels, selectChannel);
    }

    public static string ApplyPolicy(
        string text,
        ChatSelectChannel channel,
        ChatSelectChannel allowedChannels)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        return IsAllowed(allowedChannels, channel)
            ? ReplaceAliases(text)
            : StripDirectEmoji(text);
    }

    public static string ApplyPolicy(
        string text,
        ChatChannel channel,
        ChatSelectChannel allowedChannels)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        return IsAllowed(allowedChannels, channel)
            ? ReplaceAliases(text)
            : StripDirectEmoji(text);
    }

    public static string ReplaceAliases(string text, IPrototypeManager? prototypeManager = null)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        return AliasRegex.Replace(text, match =>
        {
            return TryGet(match.Groups[1].Value, prototypeManager, out var definition) &&
                   definition.HasDirectValue
                ? definition.Value
                : match.Value;
        });
    }

    public static string ReplaceAliases(
        string text,
        int cursorPosition,
        IPrototypeManager? prototypeManager,
        out int newCursorPosition)
    {
        newCursorPosition = Math.Clamp(cursorPosition, 0, text.Length);
        if (string.IsNullOrEmpty(text))
            return text;

        var matches = AliasRegex.Matches(text);
        if (matches.Count == 0)
            return text;

        var builder = new StringBuilder(text.Length);
        var sourceIndex = 0;
        var changed = false;

        foreach (Match match in matches)
        {
            builder.Append(text, sourceIndex, match.Index - sourceIndex);
            sourceIndex = match.Index + match.Length;

            if (!TryGet(match.Groups[1].Value, prototypeManager, out var emoji) ||
                !emoji.HasDirectValue)
            {
                builder.Append(match.Value);
                continue;
            }

            changed = true;
            builder.Append(emoji.Value);

            var matchEnd = match.Index + match.Length;
            if (newCursorPosition >= matchEnd)
                newCursorPosition += emoji.Value.Length - match.Length;
            else if (newCursorPosition > match.Index)
                newCursorPosition = builder.Length;
        }

        if (!changed)
            return text;

        if (sourceIndex < text.Length)
            builder.Append(text, sourceIndex, text.Length - sourceIndex);

        newCursorPosition = Math.Clamp(newCursorPosition, 0, builder.Length);
        return builder.ToString();
    }

    public static bool TryMatchAlias(
        string text,
        int index,
        int endExclusive,
        IPrototypeManager? prototypeManager,
        out ChatEmojiDefinition definition,
        out int consumedLength)
    {
        definition = default;
        consumedLength = 0;

        if (index < 0 ||
            index >= endExclusive ||
            endExclusive > text.Length ||
            text[index] != ':')
        {
            return false;
        }

        var end = text.IndexOf(':', index + 1, endExclusive - index - 1);
        if (end <= index + 1)
            return false;

        var aliasSpan = text.AsSpan(index + 1, end - index - 1);
        if (!IsValidAlias(aliasSpan) ||
            !TryGet(aliasSpan.ToString(), prototypeManager, out definition))
        {
            return false;
        }

        consumedLength = end - index + 1;
        return true;
    }

    public static string StripDirectEmoji(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        var builder = new StringBuilder(text.Length);
        foreach (var rune in text.EnumerateRunes())
        {
            if (IsSupplementalEmojiRune(rune) ||
                IsRegionalIndicator(rune) ||
                IsEmojiModifier(rune) ||
                IsKeycap(rune) ||
                IsVariationSelector(rune) ||
                IsZeroWidthJoiner(rune) ||
                KnownEmojiRunes.Contains(rune.Value))
            {
                continue;
            }

            builder.Append(rune);
        }

        return builder.ToString();
    }

    public static bool TryMapChannel(ChatChannel channel, out ChatSelectChannel selectChannel)
    {
        selectChannel = channel switch
        {
            ChatChannel.Local => ChatSelectChannel.Local,
            ChatChannel.Whisper => ChatSelectChannel.Whisper,
            ChatChannel.Radio => ChatSelectChannel.Radio,
            ChatChannel.LOOC => ChatSelectChannel.LOOC,
            ChatChannel.OOC => ChatSelectChannel.OOC,
            ChatChannel.Emotes => ChatSelectChannel.Emotes,
            ChatChannel.Dead => ChatSelectChannel.Dead,
            ChatChannel.Admin or ChatChannel.AdminAlert or ChatChannel.AdminChat => ChatSelectChannel.Admin,
            ChatChannel.Telepathic => ChatSelectChannel.Telepathic,
            ChatChannel.CollectiveMind => ChatSelectChannel.CollectiveMind,
            _ => ChatSelectChannel.None
        };

        return selectChannel != ChatSelectChannel.None;
    }

    private static IEnumerable<ChatEmojiDefinition> EnumerateCustomDefinitions(
        IPrototypeManager prototypeManager)
    {
        foreach (var customEmoji in prototypeManager.EnumeratePrototypes<ChatCustomEmojiPrototype>())
        {
            if (!IsValidAlias(customEmoji.ID.AsSpan()))
                continue;

            yield return CreateCustomDefinition(customEmoji);
        }
    }

    private static ChatEmojiDefinition CreateCustomDefinition(ChatCustomEmojiPrototype prototype)
    {
        return new ChatEmojiDefinition(
            prototype.ID,
            string.Empty,
            ChatEmojiCategory.Custom,
            prototype.RsiPath,
            string.IsNullOrWhiteSpace(prototype.State) ? prototype.ID : prototype.State);
    }

    private static Dictionary<string, ChatEmojiDefinition> BuildAliasMap()
    {
        var map = new Dictionary<string, ChatEmojiDefinition>();
        foreach (var definition in Definitions)
        {
            map[definition.Alias.ToLowerInvariant()] = definition;
        }

        return map;
    }

    private static HashSet<int> BuildKnownEmojiRuneSet()
    {
        var values = new HashSet<int>();
        foreach (var definition in Definitions)
        {
            foreach (var rune in definition.Value.EnumerateRunes())
            {
                if (!IsVariationSelector(rune) && !IsZeroWidthJoiner(rune) && !IsKeycap(rune))
                    values.Add(rune.Value);
            }
        }

        return values;
    }

    private static bool IsValidAlias(ReadOnlySpan<char> alias)
    {
        if (alias.Length == 0)
            return false;

        foreach (var ch in alias)
        {
            if (!char.IsAsciiLetterOrDigit(ch) && ch is not '_' and not '+' and not '-')
                return false;
        }

        return true;
    }

    private static bool ContainsEmojiRune(string textElement)
    {
        foreach (var rune in textElement.EnumerateRunes())
        {
            if (IsSupplementalEmojiRune(rune) ||
                IsRegionalIndicator(rune) ||
                IsEmojiModifier(rune) ||
                IsKeycap(rune) ||
                KnownEmojiRunes.Contains(rune.Value))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsSupplementalEmojiRune(Rune rune)
    {
        return rune.Value is >= 0x1F000 and <= 0x1FAFF;
    }

    private static bool IsRegionalIndicator(Rune rune)
    {
        return rune.Value is >= 0x1F1E6 and <= 0x1F1FF;
    }

    private static bool IsVariationSelector(Rune rune)
    {
        return rune.Value is >= 0xFE00 and <= 0xFE0F or >= 0xE0100 and <= 0xE01EF;
    }

    private static bool IsEmojiModifier(Rune rune)
    {
        return rune.Value is >= 0x1F3FB and <= 0x1F3FF;
    }

    private static bool IsZeroWidthJoiner(Rune rune)
    {
        return rune.Value == 0x200D;
    }

    private static bool IsKeycap(Rune rune)
    {
        return rune.Value == 0x20E3;
    }
}
