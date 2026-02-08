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

    public static event Action<UIStoreSlot, ItemSO> OnStoreSlotSelected;

    private void Start()
    {
        iconImage = GetComponent<Image>();
        
        iconImage.sprite = shopItem.item_sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (FMODEvents.Instance != null)
                PlayUiOneShot(FMODEvents.Instance.uiClick);
        }

        if (shopItem != null && !soldOut)
        {
            OnStoreSlotSelected?.Invoke(this, shopItem);
        }
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
