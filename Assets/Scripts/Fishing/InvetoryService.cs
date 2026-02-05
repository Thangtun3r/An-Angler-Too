public static class InventoryService
{
    //this script acts as a global access point for the player's inventory 
    //register it with player or else booom.
    public static FishInventory PlayerInventory { get; private set; }

    public static void RegisterPlayerInventory(FishInventory inventory)
    {
        PlayerInventory = inventory;
    }

    public static bool AddToPlayer(ItemSO item)
    {
        if (PlayerInventory == null) return false;
        return PlayerInventory.AddItem(item);
    }

    public static bool RemoveFromPlayer(ItemSO item)
    {
        if (PlayerInventory == null) return false;
        return PlayerInventory.RemoveItem(item);
    }
}