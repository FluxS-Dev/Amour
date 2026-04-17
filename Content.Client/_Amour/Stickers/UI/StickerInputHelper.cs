using Content.Shared._Amour.Stickers;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._Amour.Stickers.UI;

public static class StickerInputHelper
{
    public static void InsertSticker(LineEdit input, StickerPrototype sticker)
    {
        input.InsertAtCursor($"#{sticker.ID}# ");
        input.GrabKeyboardFocus();
    }
}
