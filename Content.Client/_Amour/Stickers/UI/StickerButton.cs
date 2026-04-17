using System;
using System.Numerics;
using Robust.Client.UserInterface.Controls;
using Robust.Client.ResourceManagement;
using Robust.Client.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Utility;
using Robust.Shared.Prototypes;
using Robust.Shared.Localization;
using Content.Shared._Amour.Stickers;

namespace Content.Client._Amour.Stickers.UI;

public sealed class StickerButton : Button
{
    public event Action<StickerPrototype>? OnStickerSelected;

    public StickerButton()
    {
        var resCache = IoCManager.Resolve<IResourceCache>();
        var prototypeManager = IoCManager.Resolve<IPrototypeManager>();
        
        Texture? texture = null;
        
        if (prototypeManager.TryIndex("laugh", out StickerPrototype? sticker) &&
            resCache.TryGetResource<TextureResource>(sticker.TexturePath, out var texRes))
        {
            texture = texRes.Texture;
        }

        if (texture != null)
        {
            var texRect = new TextureRect
            {
                Texture = texture,
                Stretch = TextureRect.StretchMode.KeepAspectCentered,
                HorizontalExpand = false,
                VerticalExpand = false,
                MinSize = new Vector2(24, 24),
                Margin = new Thickness(2)
            };
            AddChild(texRect);
        }
        else
        {
            Text = ":)";
        }

        SetSize = new Vector2(30, 30);
        HorizontalExpand = false;
        VerticalExpand = false;
        ToolTip = Loc.GetString("stickers-button-tooltip");
        OnPressed += OnClick;
    }

    private void OnClick(ButtonEventArgs args)
    {
        var window = StickerSelectorWindow.GetInstance();
        window.ClearHandlers();
        window.OnStickerSelected += s => OnStickerSelected?.Invoke(s);
        window.OpenCentered();
    }
}
