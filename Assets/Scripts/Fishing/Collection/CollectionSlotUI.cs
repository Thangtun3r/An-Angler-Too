using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;
using FMODUnity;

public class CollectionSlotUI : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("UI")]
    public Image icon;
    public TMP_Text nameText;

    [Header("Locked Visuals")]
    public string lockedName = "???";

    [Tooltip("Locked tint color (#3B517C)")]
    public Color lockedTint = new Color(0x3B / 255f, 0x51 / 255f, 0x7C / 255f, 1f);

    [Header("Hover")]
    public float hoverScale = 1.2f;
    public float hoverDuration = 0.12f;
    public Ease hoverEase = Ease.OutBack;

    [Header("Click")]
    public float clickScale = 0.9f;
    public float clickInDuration = 0.06f;
    public float clickOutDuration = 0.10f;
    public Ease clickInEase = Ease.OutQuad;
    public Ease clickOutEase = Ease.OutBack;

    private ItemSO data;
    private CollectionManager manager;

    private RectTransform tweenTarget;
    private Vector3 baseScale;
    private Tween scaleTween;
    private bool isHovered;

    public void Setup(ItemSO item, CollectionManager collectionManager)
    {
        data = item;
        manager = collectionManager;
        if (icon != null)
            tweenTarget = icon.rectTransform;
        else
            tweenTarget = (RectTransform)transform;

        baseScale = tweenTarget.localScale;

        Refresh();

        if (manager != null)
            manager.OnFishDiscovered += OnFishDiscovered;
    }

    private void OnDisable()
    {
        scaleTween?.Kill();

        if (tweenTarget != null)
            tweenTarget.localScale = baseScale;

        isHovered = false;
    }

    private void OnDestroy()
    {
        if (manager != null)
            manager.OnFishDiscovered -= OnFishDiscovered;

        scaleTween?.Kill();
    }

    private void OnFishDiscovered(ItemSO item)
    {
        if (item == null || data == null) return;
        if (item.itemID == data.itemID)
            Refresh();
    }


    private void Refresh()
    {
        if (data == null) return;

        bool unlocked = manager != null && manager.IsUnlocked(data);

        if (icon != null)
        {
            icon.sprite = data.item_sprite;
            icon.color = unlocked ? Color.white : lockedTint;
        }

        if (nameText != null)
            nameText.text = unlocked ? data.item_name : lockedName;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        if (FMODEvents.Instance != null)
            PlayUiOneShot(FMODEvents.Instance.uiHover);
        ScaleTo(baseScale * hoverScale, hoverDuration, hoverEase);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        ScaleTo(baseScale, hoverDuration, hoverEase);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (manager == null || data == null) return;

        scaleTween?.Kill();

        Vector3 returnScale = isHovered
            ? baseScale * hoverScale
            : baseScale;

        Sequence seq = DOTween.Sequence();
        seq.SetUpdate(true);
        seq.Append(
            tweenTarget.DOScale(baseScale * clickScale, clickInDuration)
                       .SetEase(clickInEase)
        );
        seq.Append(
            tweenTarget.DOScale(returnScale, clickOutDuration)
                       .SetEase(clickOutEase)
        );

        scaleTween = seq;

        manager.ShowInfo(data);
    }
    
    private void ScaleTo(Vector3 target, float duration, Ease ease)
    {
        if (tweenTarget == null) return;

        scaleTween?.Kill();
        scaleTween = tweenTarget
            .DOScale(target, duration)
            .SetEase(ease)
            .SetUpdate(true);
    }

    private void PlayUiOneShot(EventReference evt)
    {
        if (AudioManager.Instance == null || FMODEvents.Instance == null)
            return;

        AudioManager.Instance.PlayOneShot(evt, transform.position);
    }
}
