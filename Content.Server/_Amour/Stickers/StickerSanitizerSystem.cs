using System.Linq;
using System.Text.RegularExpressions;
using Content.Shared._Amour.Stickers;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Robust.Shared.Log;

namespace Content.Server._Amour.Stickers;

public sealed class StickerSanitizerSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    private static readonly Regex TexRegex = new(@"\[tex\s+path\s*=\s*""(.+?)""\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex StickerRegex = new(@"#([a-zA-Z0-9_\-\.]+)#", RegexOptions.Compiled);

    private HashSet<string>? _validTexturePaths;

    public override void Initialize()
    {
        base.Initialize();
        _prototypeManager.PrototypesReloaded += OnPrototypesReloaded;
        CacheValidTextures();
    }

    public override void Shutdown()
    {
        base.Shutdown();
        _prototypeManager.PrototypesReloaded -= OnPrototypesReloaded;
    }

    private void OnPrototypesReloaded(PrototypesReloadedEventArgs obj)
    {
        CacheValidTextures();
    }

    private void CacheValidTextures()
    {
        _validTexturePaths = _prototypeManager
            .EnumeratePrototypes<StickerPrototype>()
            .Select(s => s.TexturePath.ToString())
            .ToHashSet();
    }

    public string SanitizeMessageWithStickers(string message)
    {
        var escaped = FormattedMessage.EscapeText(message);

        escaped = StickerRegex.Replace(escaped, match =>
        {
            var stickerId = match.Groups[1].Value;
            if (_prototypeManager.TryIndex<StickerPrototype>(stickerId, out var sticker))
            {
                return $"[tex path=\"{sticker.TexturePath}\"]";
            }
            return string.Empty; // Remove invalid sticker tags
        });

        var processed = TexRegex.Replace(escaped, match =>
        {
            var texturePath = match.Groups[1].Value;
            
            if (IsValidStickerTexture(texturePath))
            {
                return $"[tex path=\"{texturePath}\"]";
            }

            // Remove invalid tex tags for security
            Logger.WarningS("stickers", $"Attempted to use invalid texture path: {texturePath}");
            return string.Empty;
        });

        return processed;
    }

    private bool IsValidStickerTexture(string path)
    {
        return _validTexturePaths?.Contains(path) ?? false;
    }
}
