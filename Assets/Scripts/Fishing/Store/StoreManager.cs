using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class StoreManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public TextMeshProUGUI itemPriceText;

    [Header("Feedback")]
    [SerializeField] private NotEnoughFihFeedback notEnoughFeedback;

    private UIStoreSlot currentStoreSlot;
    private FishInventory playerInventory;

    private void Awake()
    {
        UIStoreSlot.OnStoreSlotSelected += DisplayItemDetails;
    }

    private void OnDestroy()
    {
        UIStoreSlot.OnStoreSlotSelected -= DisplayItemDetails;
    }

    private void Start()
    {
        playerInventory = FindObjectOfType<PlayerInventory>().Inventory;
    }

    // ---------------- Display ----------------

    public void DisplayItemDetails(UIStoreSlot storeSlot, ItemSO item)
    {
        currentStoreSlot = storeSlot;

        itemNameText.text = item.item_name;
        itemDescriptionText.text = item.ItemDescription;
        itemPriceText.text = BuildPriceText(storeSlot);
    }

    // ---------------- Buy ----------------

    public void BuyItem()
    {
        if (currentStoreSlot == null) return;
        if (currentStoreSlot.soldOut) return;

        if (!CanAfford(playerInventory, currentStoreSlot))
        {
            notEnoughFeedback?.Play(); // 👈 ONLY ADDITION
            return;
        }

        PayCost(playerInventory, currentStoreSlot);

        if (playerInventory.AddItem(currentStoreSlot.shopItem))
        {
            currentStoreSlot.soldOut = true;
            currentStoreSlot.iconImage.color = Color.gray;
        }
    }

    // ---------------- Cost Logic ----------------

    bool CanAfford(FishInventory inv, UIStoreSlot slot)
    {
        if (slot.acceptAnyFish)
            return CountUniqueFish(inv) >= slot.anyFishAmount;

        foreach (var cost in slot.costs)
        {
            if (CountItem(inv, cost.fish) < cost.amount)
                return false;
        }

        return true;
    }

    void PayCost(FishInventory inv, UIStoreSlot slot)
    {
        if (slot.acceptAnyFish)
        {
            RemoveUniqueFish(inv, slot.anyFishAmount);
            return;
        }

        foreach (var cost in slot.costs)
        {
            RemoveItemAmount(inv, cost.fish, cost.amount);
        }
    }

    int CountItem(FishInventory inv, ItemSO item)
    {
        int count = 0;

        for (int i = 0; i < inv.SlotCount; i++)
        {
            var it = inv.GetItem(i);
            if (it != null && !it.isQuestItem && it == item)
                count++;
        }

        return count;
    }

    int CountUniqueFish(FishInventory inv)
    {
        HashSet<ItemSO> unique = new HashSet<ItemSO>();

        for (int i = 0; i < inv.SlotCount; i++)
        {
            var fish = inv.GetItem(i);
            if (fish != null && !fish.isQuestItem)
                unique.Add(fish);
        }

        return unique.Count;
    }

    void RemoveItemAmount(FishInventory inv, ItemSO item, int amount)
    {
        for (int i = 0; i < inv.SlotCount && amount > 0; i++)
        {
            var it = inv.GetItem(i);
            if (it != null && !it.isQuestItem && it == item)
            {
                inv.RemoveAt(i);
                amount--;
            }
        }
    }

    void RemoveUniqueFish(FishInventory inv, int amount)
    {
        HashSet<ItemSO> removedTypes = new HashSet<ItemSO>();

        for (int i = 0; i < inv.SlotCount && removedTypes.Count < amount; i++)
        {
            var fish = inv.GetItem(i);

            if (fish != null && !fish.isQuestItem && !removedTypes.Contains(fish))
            {
                inv.RemoveAt(i);
                removedTypes.Add(fish);
            }
        }
    }

    string BuildPriceText(UIStoreSlot slot)
    {
        if (slot.acceptAnyFish)
            return $"Cost:\n{slot.anyFishAmount}x Different fih";

        string text = "Cost:\n";

        foreach (var cost in slot.costs)
        {
            text += $"{cost.amount}x {cost.fish.item_name}\n";
        }

        return text;
    }
}
