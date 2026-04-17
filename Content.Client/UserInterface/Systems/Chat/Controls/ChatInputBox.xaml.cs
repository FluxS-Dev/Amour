// SPDX-FileCopyrightText: 2022 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Jezithyr <Jezithyr.@gmail.com>
// SPDX-FileCopyrightText: 2022 Jezithyr <Jezithyr@gmail.com>
// SPDX-FileCopyrightText: 2022 Jezithyr <jmaster9999@gmail.com>
// SPDX-FileCopyrightText: 2022 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 exincore <me@exin.xyz>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 wrexbe <wrexbe@protonmail.com>
// SPDX-FileCopyrightText: 2023 LordCarve <27449516+LordCarve@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 ShadowCommander <10494922+ShadowCommander@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Sk1tch <ben.peter.smith@gmail.com>
// SPDX-FileCopyrightText: 2024 Your Name <you@example.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using Content.Client.Stylesheets;
using Content.Shared.Chat;
using Content.Shared.Input;
using Robust.Client.UserInterface.Controls;
using Content.Client._Amour.Stickers.UI;
using Content.Shared.Chat;

namespace Content.Client.UserInterface.Systems.Chat.Controls;

[Virtual]
public class ChatInputBox : PanelContainer
{
    public readonly ChannelSelectorButton ChannelSelector;
    public readonly HistoryLineEdit Input;
    public readonly ChannelFilterButton FilterButton;
    public readonly StickerButton StickerButton;
    protected readonly BoxContainer Container;
    protected ChatChannel ActiveChannel { get; private set; } = ChatChannel.Local;

    public ChatInputBox()
    {
        Container = new BoxContainer
        {
            Orientation = BoxContainer.LayoutOrientation.Horizontal,
            SeparationOverride = 4
        };
        AddChild(Container);

        ChannelSelector = new ChannelSelectorButton
        {
            Name = "ChannelSelector",
            ToggleMode = true,
            StyleClasses = {"chatSelectorOptionButton"},
            MinWidth = 75
        };
        Container.AddChild(ChannelSelector);
        Input = new HistoryLineEdit
        {
            Name = "Input",
            PlaceHolder = GetChatboxInfoPlaceholder(),
            HorizontalExpand = true,
            StyleClasses = {"chatLineEdit"}
        };
        Container.AddChild(Input);

        // Amour edit start
        StickerButton = new StickerButton
        {
            Name = "StickerButton",
            Visible = false,
            StyleClasses = {"chatSelectorOptionButton"}
        };
        StickerButton.OnStickerSelected += sticker => 
        {
            // Insert sticker if in OOC or Admin channels
            if (ActiveChannel == ChatChannel.OOC || ActiveChannel == ChatChannel.Admin || ActiveChannel == ChatChannel.AdminChat)
                StickerInputHelper.InsertSticker(Input, sticker);
        };
        Container.AddChild(StickerButton);
        // Amour edit end

        FilterButton = new ChannelFilterButton
        {
            Name = "FilterButton",
            StyleClasses = {"chatFilterOptionButton"}
        };
        Container.AddChild(FilterButton);
        AddStyleClass(StyleNano.StyleClassChatSubPanel);
        ChannelSelector.OnChannelSelect += UpdateActiveChannel;
    }

    private void UpdateActiveChannel(ChatSelectChannel selectedChannel)
    {
        ActiveChannel = (ChatChannel) selectedChannel;
        // Amour edit: Stickers are available in OOC and Admin chat
        StickerButton.Visible = (ActiveChannel == ChatChannel.OOC || ActiveChannel == ChatChannel.Admin || ActiveChannel == ChatChannel.AdminChat);
    }

    private static string GetChatboxInfoPlaceholder()
    {
        return (BoundKeyHelper.IsBound(ContentKeyFunctions.FocusChat), BoundKeyHelper.IsBound(ContentKeyFunctions.CycleChatChannelForward)) switch
        {
            (true, true) => Loc.GetString("hud-chatbox-info", ("talk-key", BoundKeyHelper.ShortKeyName(ContentKeyFunctions.FocusChat)), ("cycle-key", BoundKeyHelper.ShortKeyName(ContentKeyFunctions.CycleChatChannelForward))),
            (true, false) => Loc.GetString("hud-chatbox-info-talk", ("talk-key", BoundKeyHelper.ShortKeyName(ContentKeyFunctions.FocusChat))),
            (false, true) => Loc.GetString("hud-chatbox-info-cycle", ("cycle-key", BoundKeyHelper.ShortKeyName(ContentKeyFunctions.CycleChatChannelForward))),
            (false, false) => Loc.GetString("hud-chatbox-info-unbound")
        };
    }
}