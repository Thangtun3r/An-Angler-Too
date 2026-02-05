using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectionManager : MonoBehaviour
{
    public static CollectionManager Instance;

    public Image infoIcon;
    public TMP_Text infoName;
    public TMP_Text infoDesc;

    public Sprite lockedSprite;

    private HashSet<string> discovered = new HashSet<string>();

    public event System.Action<ItemSO> OnFishDiscovered;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        InventoryService.OnItemAdded += HandleItemAdded;
    }

    private void OnDisable()
    {
        InventoryService.OnItemAdded -= HandleItemAdded;
    }

    private void HandleItemAdded(ItemSO item)
    {
        if (discovered.Contains(item.itemID))
            return;

        discovered.Add(item.itemID);
        OnFishDiscovered?.Invoke(item);
    }

    public void GetDisplaySprite(ItemSO item, out Sprite sprite)
    {
        bool unlocked = discovered.Contains(item.itemID);
        sprite = unlocked ? item.item_sprite : lockedSprite;
    }

    public void ShowInfo(ItemSO item)
    {
        bool unlocked = discovered.Contains(item.itemID);

        if (unlocked)
        {
            infoIcon.sprite = item.item_sprite;
            infoName.text = item.item_name;
            infoDesc.text = item.ItemDescription;
        }
        else
        {
            infoIcon.sprite = lockedSprite;
            infoName.text = "???";
            infoDesc.text = item.HintDescription;
        }
    }

    [ContextMenu("DEBUG Unlock All Fish")]
    public void DebugUnlockAllFish()
    {
        CollectionUIBuilder builder = FindObjectOfType<CollectionUIBuilder>();

        foreach (var fish in builder.allFish)
        {
            if (discovered.Contains(fish.itemID)) continue;

            discovered.Add(fish.itemID);
            OnFishDiscovered?.Invoke(fish);
        }

        Debug.Log("All fish unlocked for testing.");
    }
}