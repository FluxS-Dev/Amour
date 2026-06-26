namespace Content.Shared.Inventory;

public partial class InventorySystem
{
    private static readonly TimeSpan QuickSelfUnequipDelay = TimeSpan.FromSeconds(1);
    private static readonly TimeSpan JumpsuitSelfUnequipDelay = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan OuterClothingSelfUnequipDelay = TimeSpan.FromSeconds(3);

    private static TimeSpan GetUnequipDelay(
        EntityUid actor,
        EntityUid target,
        SlotFlags slotFlags,
        TimeSpan itemDelay)
    {
        if (actor != target)
            return itemDelay;

        var slotDelay = slotFlags switch
        {
            SlotFlags.BACK or SlotFlags.GLOVES or SlotFlags.FEET => QuickSelfUnequipDelay,
            SlotFlags.INNERCLOTHING => JumpsuitSelfUnequipDelay,
            SlotFlags.OUTERCLOTHING => OuterClothingSelfUnequipDelay,
            _ => TimeSpan.Zero,
        };

        return itemDelay > slotDelay ? itemDelay : slotDelay;
    }
}
