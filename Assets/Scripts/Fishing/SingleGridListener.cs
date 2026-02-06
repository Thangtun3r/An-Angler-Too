using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class SingleGridListener : MonoBehaviour
{
    [Header("References")]
    public Canvas targetCanvas;

    [Header("Motion")]
    public float snapDuration = 0.12f;
    public Ease snapEase = Ease.OutQuad;

    [Header("Size Matching")]
    public bool matchSize = true;
    public Vector2 sizePadding = Vector2.zero;

    private RectTransform selfRect;
    private RectTransform currentTarget;
    private EventSystem eventSystem;

    void Awake()
    {
        selfRect = GetComponent<RectTransform>();
        eventSystem = EventSystem.current;

        if (targetCanvas == null)
            targetCanvas = GetComponentInParent<Canvas>();
    }

    void Update()
    {
        UpdateHover();
    }
    

    void UpdateHover()
    {
        if (eventSystem == null) return;

        PointerEventData data = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        eventSystem.RaycastAll(data, results);

        foreach (var r in results)
        {
            UIFishSlot slot = r.gameObject.GetComponent<UIFishSlot>();
            if (slot == null) continue;

            RectTransform targetRect = slot.GetComponent<RectTransform>();
            if (targetRect == currentTarget) return;

            SnapTo(targetRect);
            return;
        }

        currentTarget = null;
    }
    
    void SnapTo(RectTransform target)
    {
        currentTarget = target;

        selfRect.DOKill();

        Vector2 localPoint;
        RectTransform parentRect = selfRect.parent as RectTransform;
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            RectTransformUtility.WorldToScreenPoint(targetCanvas.worldCamera, target.position),
            targetCanvas.worldCamera,
            out localPoint
        );

        selfRect.DOAnchorPos(localPoint, snapDuration)
            .SetEase(snapEase)
            .SetUpdate(true);

        if (matchSize)
        {
            selfRect.DOSizeDelta(
                target.sizeDelta + sizePadding,
                snapDuration
            ).SetEase(snapEase);
        }
    }
}
