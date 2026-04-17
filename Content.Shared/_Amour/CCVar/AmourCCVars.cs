using Robust.Shared.Configuration;

namespace Content.Shared._Amour.CCVar;

[CVarDefs]
public sealed class AmourCCVars
{
    /// <summary>
    ///     URL of the Discord bot API for receiving OOC messages from Discord.
    /// </summary>
    public static readonly CVarDef<string> DiscordOocBotApiUrl =
        CVarDef.Create("discord.ooc_bot_api_url", string.Empty, CVar.SERVERONLY | CVar.CONFIDENTIAL);

    /// <summary>
    ///     Password for Discord bot OOC API authentication.
    /// </summary>
    public static readonly CVarDef<string> DiscordOocBotApiPassword =
        CVarDef.Create("discord.ooc_bot_api_password", string.Empty, CVar.SERVERONLY | CVar.CONFIDENTIAL);

    /// <summary>
    ///     URL of the Discord webhook used to send in-game OOC messages to Discord.
    /// </summary>
    public static readonly CVarDef<string> DiscordOocWebhookUrl =
        CVarDef.Create("discord.ooc_webhook_url", string.Empty, CVar.SERVERONLY | CVar.CONFIDENTIAL);

    /// <summary>
    ///     Discord bot token for embedded OOC bridge bot.
    /// </summary>
    public static readonly CVarDef<string> DiscordOocBotToken =
        CVarDef.Create("amour.discord_ooc_bot_token", string.Empty, CVar.SERVERONLY | CVar.CONFIDENTIAL);

    /// <summary>
    ///     Discord channel ID for OOC bridge.
    /// </summary>
    public static readonly CVarDef<ulong> DiscordOocChannelId =
        CVarDef.Create("amour.discord_ooc_channel_id", 0UL, CVar.SERVERONLY);
}