using System;
    
public static class InventoryService
{
    public static FishInventory PlayerInventory { get; private set; }

    public static event Action<ItemSO> OnItemAdded;

    public static void RegisterPlayerInventory(FishInventory inventory)
    {
        PlayerInventory = inventory;
    }

    public static bool AddToPlayer(ItemSO item)
    {
        if (PlayerInventory == null) return false;

        bool added = PlayerInventory.AddItem(item);

        if (added)
        {
            OnItemAdded?.Invoke(item);
        }

        return added;
    }

    public static bool RemoveFromPlayer(ItemSO item)
    {
        if (PlayerInventory == null) return false;
        if (!PlayerInventory.Contains(item)) return false;
        return PlayerInventory.RemoveItem(item);
    }
}