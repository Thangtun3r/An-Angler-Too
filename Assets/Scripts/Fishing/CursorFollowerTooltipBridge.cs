using UnityEngine;
using System.Reflection;

public class CursorFollowerTooltipBridge : MonoBehaviour
{
    public CursorItemTooltip tooltip;

    private static FieldInfo slotIndexField;
    private UIFishSlot currentSlot;

    void Awake()
    {
        if (slotIndexField == null)
        {
            slotIndexField = typeof(UIFishSlot).GetField(
                "slotIndex",
                BindingFlags.NonPublic | BindingFlags.Instance
            );
        }

        tooltip?.Hide();
    }

    void OnEnable()
    {
        UIFishSlotEvents.OnHoverEnter += OnSlotEnter;
        UIFishSlotEvents.OnHoverExit += OnSlotExit;
    }

    void OnDisable()
    {
        UIFishSlotEvents.OnHoverEnter -= OnSlotEnter;
        UIFishSlotEvents.OnHoverExit -= OnSlotExit;
    }

    void Update()
    {
        // 🔥 Drag just started → kill tooltip immediately
        if (UIFishSlot.selectedIndex != -1 && tooltip != null)
        {
            tooltip.Hide();
            currentSlot = null;
        }
    }

    private void OnSlotEnter(UIFishSlot slot)
    {
        // Ignore hover while dragging
        if (UIFishSlot.selectedIndex != -1)
            return;

        if (slot == null || slot.fishInventory == null)
            return;

        int index = (int)slotIndexField.GetValue(slot);
        var item = slot.fishInventory.GetItem(index);
        if (item == null)
            return;

        currentSlot = slot;
        tooltip.Show(item);
    }

    private void OnSlotExit(UIFishSlot slot)
    {
        if (currentSlot != slot)
            return;

        // Ignore exit spam during drag
        if (UIFishSlot.selectedIndex != -1)
            return;

        currentSlot = null;
        tooltip.Hide();
    }
}