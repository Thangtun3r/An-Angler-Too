using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using FMODUnity;

[System.Serializable]
public class FishCost
{
    public ItemSO fish;
    public int amount;
}

public class UIStoreSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    public Image iconImage;
    public Sprite iconSprite;
    public ItemSO shopItem;
    public bool soldOut = false;

    [Header("Specific Fish Costs")]
    public List<FishCost> costs = new List<FishCost>();

    [Header("Flexible Cost Mode")]
    public bool acceptAnyFish = false;
    public int anyFishAmount = 0;

    // EXISTING (used by StoreManager / popup)
    public static event Action<UIStoreSlot, ItemSO> OnStoreSlotSelected;

    // NEW (used ONLY by hand gesture)
    public static event Action<UIStoreSlot> OnStoreSlotClickedForGesture;

    private void Start()
    {
        iconImage = GetComponent<Image>();
        iconImage.sprite = shopItem.item_sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (shopItem == null || soldOut)
            return;

        // Existing behaviour (do NOT break)
        OnStoreSlotSelected?.Invoke(this, shopItem);

        // NEW: click-only gesture signal
        OnStoreSlotClickedForGesture?.Invoke(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (FMODEvents.Instance != null)
            PlayUiOneShot(FMODEvents.Instance.uiHover);
    }

    private void PlayUiOneShot(EventReference evt)
    {
        if (AudioManager.Instance == null || FMODEvents.Instance == null)
            return;

        AudioManager.Instance.PlayOneShot(evt, transform.position);
    }
}
