using UnityEngine;
using Yarn.Unity;
using Yarn.Unity.ExposedVariables;

public class CheckQuestItem : MonoBehaviour
{
    private DialogueRunner diagRunner;

void Awake() {
    diagRunner = GameObject.FindFirstObjectByType<DialogueRunner>();
}

    [YarnCommand("checkForItem")]
    public void CheckForItem(string itemName)
    {
        FishInventory inventory = InventoryService.PlayerInventory;

        ItemSO item = ItemDatabase.GetByID(itemName);
        bool hasItem = item != null && inventory.Contains(item);

        Debug.Log("set $" + itemName + " to " + hasItem);
        diagRunner.VariableStorage.SetValue("$" + itemName, hasItem);
    }
}


public static class ItemDatabase
{
    public static void Register(ItemSO item)
    {
        // No-op: inventory is the source of truth now.
    }

    public static ItemSO GetByID(string id)
    {
        FishInventory inventory = InventoryService.PlayerInventory;
        if (inventory == null) return null;

        int count = inventory.SlotCount;
        for (int i = 0; i < count; i++)
        {
            ItemSO item = inventory.GetItem(i);
            if (item != null && item.itemID == id)
                return item;
        }
        return null;
    }
}
