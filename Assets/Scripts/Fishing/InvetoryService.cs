using System;

public static class InventoryService
{
    public static FishInventory PlayerInventory { get; private set; }

    public static event Action<ItemSO> OnItemAdded;
    public static event Action<ItemSO> OnItemRemoved; // NEW

    public static void RegisterPlayerInventory(FishInventory inventory)
    {
        PlayerInventory = inventory;
    }

    public static bool AddToPlayer(ItemSO item)
    {
        if (PlayerInventory == null) return false;

        bool added = PlayerInventory.AddItem(item);

        if (added)
            OnItemAdded?.Invoke(item);

        return added;
    }
    

    public static bool RemoveFromPlayer(ItemSO item)
    {
        if (PlayerInventory == null) return false;
        if (!PlayerInventory.Contains(item)) return false;

        bool removed = PlayerInventory.RemoveItem(item);

        if (removed)
            OnItemRemoved?.Invoke(item); // NEW

        return removed;
    }
}