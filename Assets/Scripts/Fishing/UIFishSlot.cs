using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using FMODUnity;

public class UIFishSlot : MonoBehaviour,
    IPointerClickHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public Image fishImage;
    public Image backgroundVisual;
    public FishInventory fishInventory;
    public bool isDiscardSlot;

    private bool hadItemLastFrame = false;
    
    public static int selectedIndex = -1;

    [Header("Hover")]
    public float hoverScale = 1.1f;
    public float hoverDuration = 0.15f;
    public Ease hoverEase = Ease.OutBack;

    [Header("Tint")]
    public Color normalTint = Color.white;
    public Color hoverTint = new Color(1f, 1f, 1f, 0.9f);
    public float tintDuration = 0.12f;

    private int slotIndex;
    private Vector3 imageOriginalScale;

    void Awake()
    {
        imageOriginalScale = fishImage.rectTransform.localScale;
        backgroundVisual.color = normalTint;
    }

    public void Initialize(int index, FishInventory inven)
    {
        slotIndex = index;
        fishInventory = inven;
    }
    private void ApplyHoverVisuals()
    {
        if (isDiscardSlot) return;
        if (fishInventory == null) return;

        var item = fishInventory.GetItem(slotIndex);
        if (item == null) return;

        fishImage.rectTransform.DOKill();
        fishImage.rectTransform
            .DOScale(imageOriginalScale * hoverScale, hoverDuration)
            .SetEase(hoverEase);

        backgroundVisual.DOKill();
        backgroundVisual.DOColor(hoverTint, tintDuration);

        CursorFollower.Instance?.SnapTo(transform as RectTransform);
    }

    private void ApplyNormalVisuals()
    {
        fishImage.rectTransform.DOKill();
        fishImage.rectTransform
            .DOScale(imageOriginalScale, hoverDuration)
            .SetEase(Ease.OutQuad);

        backgroundVisual.DOKill();
        backgroundVisual.DOColor(normalTint, tintDuration);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIFishSlotEvents.OnHoverEnter?.Invoke(this);
        if (selectedIndex != -1)
        {
            backgroundVisual.DOKill();
            backgroundVisual.DOColor(hoverTint, tintDuration);
            return;
        }

        if (fishInventory.GetItem(slotIndex) == null) return;

        if (FMODEvents.Instance != null)
            PlayUiOneShot(FMODEvents.Instance.uiHover);

        ApplyHoverVisuals();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIFishSlotEvents.OnHoverExit?.Invoke(this);

        backgroundVisual.DOKill();
        backgroundVisual.DOColor(normalTint, tintDuration);
        
        if (selectedIndex != -1) return;

        ApplyNormalVisuals();
        CursorFollower.Instance?.Clear();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (selectedIndex == -1)
        {
            if (isDiscardSlot) return;

            var item = fishInventory.GetItem(slotIndex);
            if (item == null) return;

            selectedIndex = slotIndex;
            if (FMODEvents.Instance != null)
                PlayUiOneShot(FMODEvents.Instance.uiPickUp);

            // Hide this slot image while dragging
            fishImage.color = new Color(1, 1, 1, 0);

            CursorFollower.Instance?.Select(fishImage.sprite);
            return;
        }
        

        int fromIndex = selectedIndex;
        if (!isDiscardSlot && slotIndex == fromIndex)
        {
            selectedIndex = -1;

            fishImage.color = Color.white;

            CursorFollower.Instance?.Clear();
            ApplyHoverVisuals();
            if (FMODEvents.Instance != null)
                PlayUiOneShot(FMODEvents.Instance.uiPlaceDown);
            return;
        }
        
        if (isDiscardSlot)
        {
            var item = fishInventory.GetItem(fromIndex);
            
            if (item == null || item.isQuestItem) return;
            selectedIndex = -1;
            fishInventory.RemoveAt(fromIndex);

            CursorFollower.Instance?.Clear();
            if (FMODEvents.Instance != null)
                PlayUiOneShot(FMODEvents.Instance.uiDiscard);
            return;
        }

        selectedIndex = -1; 
        fishInventory.SwapSlots(fromIndex, slotIndex);

        CursorFollower.Instance?.Clear();
        ApplyHoverVisuals();
        if (FMODEvents.Instance != null)
            PlayUiOneShot(FMODEvents.Instance.uiPlaceDown);
    }

    public void RefreshSlot()
    {
        if (isDiscardSlot) return;

        var item = fishInventory.GetItem(slotIndex);

        fishImage.rectTransform.DOKill();
        backgroundVisual.color = normalTint;

        if (item == null)
        {
            hadItemLastFrame = false;

            fishImage.sprite = null;
            fishImage.color = new Color(1, 1, 1, 0);
            fishImage.rectTransform.localScale = imageOriginalScale;
            return;
        }

        fishImage.sprite = item.item_sprite;
        fishImage.color = Color.white;

    
        if (!hadItemLastFrame)
        {
            fishImage.rectTransform.localScale = Vector3.zero;
            fishImage.rectTransform
                .DOScale(imageOriginalScale, 0.18f)
                .SetEase(Ease.OutBack);
        }
        else
        {
            fishImage.rectTransform.localScale = imageOriginalScale;
        }

        hadItemLastFrame = true;
    }

    private void PlayUiOneShot(EventReference evt)
    {
        if (AudioManager.Instance == null || FMODEvents.Instance == null)
            return;

        AudioManager.Instance.PlayOneShot(evt, transform.position);
    }
}
