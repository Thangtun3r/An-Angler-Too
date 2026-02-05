using UnityEngine;

public class UIFishInventory : MonoBehaviour
{
    private UIFishSlot[] fishSlots;
    private FishInventory fishInventory;

    void Awake()
    {
        fishInventory = FindObjectOfType<PlayerInventory>().Inventory;

        fishSlots = GetComponentsInChildren<UIFishSlot>();

        for (int i = 0; i < fishSlots.Length; i++)
        {
            fishSlots[i].Initialize(i, fishInventory);
        }
    }

    private void OnEnable()
    {
        FishInventory.OnInventoryChanged += RefreshUI;
    }

    private void OnDisable()
    {
        FishInventory.OnInventoryChanged -= RefreshUI;
    }

    void RefreshUI()
    {
        for (int i = 0; i < fishSlots.Length; i++)
        {
            fishSlots[i].RefreshSlot();
        }
    }
}