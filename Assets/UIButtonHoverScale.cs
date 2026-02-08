using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UIButtonHoverScale : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    [SerializeField] private RectTransform target;

    [Header("Scales")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float pressScale = 0.95f;

    [Header("Timing")]
    [SerializeField] private float duration = 0.12f;

    private Vector3 defaultScale;
    private Tween tween;
    private bool isHovering;

    private void Awake()
    {
        if (target == null)
            target = transform as RectTransform;

        defaultScale = target.localScale;
    }

    // ---------------- Hover ----------------

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        AnimateScale(defaultScale * hoverScale, Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        AnimateScale(defaultScale, Ease.OutSine);
    }

    // ---------------- Press ----------------

    public void OnPointerDown(PointerEventData eventData)
    {
        AnimateScale(defaultScale * pressScale, Ease.OutSine);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Return to hover if still hovering, otherwise normal
        AnimateScale(
            isHovering ? defaultScale * hoverScale : defaultScale,
            Ease.OutBack
        );
    }

    // ---------------- Core ----------------

    void AnimateScale(Vector3 scale, Ease ease)
    {
        tween?.Kill();
        tween = target.DOScale(scale, duration).SetEase(ease);
    }
}