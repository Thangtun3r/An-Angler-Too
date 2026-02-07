using UnityEngine;

public class UIFishInventory : MonoBehaviour
{
    private UIFishSlot[] fishSlots;
    public FishInventory fishInventory;

    void Awake()
    {

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