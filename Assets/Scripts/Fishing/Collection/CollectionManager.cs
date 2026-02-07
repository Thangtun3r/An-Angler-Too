using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectionManager : MonoBehaviour
{
    public static CollectionManager Instance { get; private set; }

    [Header("UI")]
    public Image infoIcon;
    public TMP_Text infoName;
    public TMP_Text infoDesc;

    [Header("Locked")]
    public Sprite lockedSprite;

    private readonly HashSet<string> discovered = new HashSet<string>();

    public event System.Action<ItemSO> OnFishDiscovered;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
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
        if (item == null) return;
        if (string.IsNullOrEmpty(item.itemID)) return;

        if (discovered.Contains(item.itemID))
            return;

        discovered.Add(item.itemID);
        OnFishDiscovered?.Invoke(item);
    }

    public bool IsUnlocked(ItemSO item)
    {
        if (item == null) return false;
        if (string.IsNullOrEmpty(item.itemID)) return false;
        return discovered.Contains(item.itemID);
    }

    public void GetDisplaySprite(ItemSO item, out Sprite sprite)
    {
        bool unlocked = IsUnlocked(item);
        sprite = unlocked ? item.item_sprite : lockedSprite;
    }

    public void ShowInfo(ItemSO item)
    {
        if (item == null) return;

        bool unlocked = IsUnlocked(item);

        if (unlocked)
        {
            if (infoIcon) infoIcon.sprite = item.item_sprite;
            if (infoName) infoName.text = item.item_name;
            if (infoDesc) infoDesc.text = item.ItemDescription;

            if (FishPreviewRoom.Instance != null)
                FishPreviewRoom.Instance.Show(item);
        }
        else
        {
            if (infoIcon) infoIcon.sprite = lockedSprite;
            if (infoName) infoName.text = "???";
            if (infoDesc) infoDesc.text = item.HintDescription;

            if (FishPreviewRoom.Instance != null)
                FishPreviewRoom.Instance.Clear();
        }
    }

    [ContextMenu("DEBUG Unlock All Fish")]
    public void DebugUnlockAllFish()
    {
        CollectionUIBuilder builder = FindObjectOfType<CollectionUIBuilder>();
        if (builder == null || builder.allFish == null)
        {
            Debug.LogWarning("No CollectionUIBuilder / allFish found.");
            return;
        }

        foreach (var fish in builder.allFish)
        {
            if (fish == null) continue;
            if (string.IsNullOrEmpty(fish.itemID)) continue;

            if (discovered.Contains(fish.itemID)) continue;

            discovered.Add(fish.itemID);
            OnFishDiscovered?.Invoke(fish);
        }

        Debug.Log("All fish unlocked for testing.");
    }
}
