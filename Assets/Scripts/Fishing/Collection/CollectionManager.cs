using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FMODUnity;

public class CollectionManager : MonoBehaviour
{
    public static CollectionManager Instance { get; private set; }

    public Image infoIcon;
    public TMP_Text infoName;
    public TMP_Text infoDesc;

    public TMP_Text collectionProgressText;

    public RawImage fishPreviewImage;
    public Color previewLockedTint = Color.black;
    public Color previewUnlockedTint = Color.white;

    public Sprite lockedSprite;

    private readonly HashSet<string> discovered = new HashSet<string>();

    public event System.Action<ItemSO> OnFishDiscovered;

    private CollectionUIBuilder uiBuilder;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        uiBuilder = FindObjectOfType<CollectionUIBuilder>();
        UpdateProgressText();
    }

    private void OnEnable()
    {
        InventoryService.OnItemAdded += HandleItemAdded;
        if (FMODEvents.Instance != null)
            PlayUiOneShot(FMODEvents.Instance.uiPageTurn);
    }

    private void OnDisable()
    {
        InventoryService.OnItemAdded -= HandleItemAdded;
    }

    private void HandleItemAdded(ItemSO item)
    {
        if (item == null) return;
        if (string.IsNullOrEmpty(item.itemID)) return;
        if (discovered.Contains(item.itemID)) return;

        discovered.Add(item.itemID);
        OnFishDiscovered?.Invoke(item);
        UpdateProgressText();
    }

    public bool IsUnlocked(ItemSO item)
    {
        if (item == null) return false;
        if (string.IsNullOrEmpty(item.itemID)) return false;
        return discovered.Contains(item.itemID);
    }

    public void GetDisplaySprite(ItemSO item, out Sprite sprite)
    {
        sprite = IsUnlocked(item) ? item.item_sprite : lockedSprite;
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

            FishPreviewRoom.Instance?.Show(item);
            if (fishPreviewImage) fishPreviewImage.color = previewUnlockedTint;
        }
        else
        {
            if (infoIcon) infoIcon.sprite = lockedSprite;
            if (infoName) infoName.text = "???";
            if (infoDesc) infoDesc.text = item.HintDescription;

            FishPreviewRoom.Instance?.Show(item);
            if (fishPreviewImage) fishPreviewImage.color = previewLockedTint;
        }
    }

    private void UpdateProgressText()
    {
        if (collectionProgressText == null || uiBuilder == null || uiBuilder.allFish == null) return;

        int collected = discovered.Count;
        int total = uiBuilder.allFish.Count;

        collectionProgressText.text = $"{collected}/{total} Fih collected!";
    }

    [ContextMenu("DEBUG Unlock All Fish")]
    public void DebugUnlockAllFish()
    {
        if (uiBuilder == null || uiBuilder.allFish == null) return;

        foreach (var fish in uiBuilder.allFish)
        {
            if (fish == null) continue;
            if (string.IsNullOrEmpty(fish.itemID)) continue;
            if (discovered.Contains(fish.itemID)) continue;

            discovered.Add(fish.itemID);
            OnFishDiscovered?.Invoke(fish);
        }

        UpdateProgressText();
    }

    private void PlayUiOneShot(EventReference evt)
    {
        if (AudioManager.Instance == null || FMODEvents.Instance == null)
            return;

        AudioManager.Instance.PlayOneShot(evt, transform.position);
    }
}
