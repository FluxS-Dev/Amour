using Content.Shared.Chat;
using Robust.Shared.Configuration;

namespace Content.Shared.CCVar;

public sealed partial class CCVars
{
    /// <summary>
    ///     Chat channels where direct emoji and emoji aliases are enabled.
    /// </summary>
    public static readonly CVarDef<string> ChatEmojiAllowedChannels =
        CVarDef.Create(
            "chat.emoji_allowed_channels",
            ChatEmoji.DefaultAllowedChannelsCVar,
            CVar.SERVER | CVar.REPLICATED,
            "Comma-separated list of chat channels where emoji are enabled. Example: LOOC,OOC,Dead,Admin");
}
