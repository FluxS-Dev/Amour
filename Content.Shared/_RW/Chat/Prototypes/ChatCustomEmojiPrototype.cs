using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared.Chat.Prototypes;

[Prototype("chatCustomEmoji")]
public sealed partial class ChatCustomEmojiPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField(required: true)]
    public ResPath RsiPath;

    [DataField]
    public string? State;
}
