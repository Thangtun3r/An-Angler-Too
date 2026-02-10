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

public class UIStoreSlot : MonoBehaviour,
    IPointerClickHandler,
    IPointerEnterHandler
{
    [Header("Item")]
    public Image iconImage;
    public ItemSO shopItem;

    [Header("Purchase Rules")]
    public bool allowRepeatPurchase = false;
    [HideInInspector] public bool soldOut = false;

    [Header("Specific Fish Costs")]
    public List<FishCost> costs = new List<FishCost>();

    [Header("Flexible Cost Mode")]
    public bool acceptAnyFish = false;
    public int anyFishAmount = 0;

    // Existing (used by StoreManager / popup)
    public static event Action<UIStoreSlot, ItemSO> OnStoreSlotSelected;

    // Used for gesture / hand feedback
    public static event Action<UIStoreSlot> OnStoreSlotClickedForGesture;

    private void Start()
    {
        iconImage = GetComponent<Image>();
        iconImage.sprite = shopItem.item_sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (shopItem == null)
            return;

        if (soldOut && !allowRepeatPurchase)
            return;

        OnStoreSlotSelected?.Invoke(this, shopItem);
        OnStoreSlotClickedForGesture?.Invoke(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (FMODEvents.Instance != null)
            PlayUiOneShot(FMODEvents.Instance.uiHover);
    }

    public void MarkSoldOut()
    {
        if (allowRepeatPurchase)
            return;

        soldOut = true;
        if (iconImage != null)
            iconImage.color = Color.gray;
    }

    private void PlayUiOneShot(EventReference evt)
    {
        if (AudioManager.Instance == null || FMODEvents.Instance == null)
            return;

        AudioManager.Instance.PlayOneShot(evt, transform.position);
    }
}