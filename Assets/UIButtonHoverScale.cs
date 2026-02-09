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

    private UIScaleInOnEnable scaleOwner;
    private Tween tween;
    private bool isHovering;

    private void Awake()
    {
        if (target == null)
            target = transform as RectTransform;

        scaleOwner = target.GetComponent<UIScaleInOnEnable>();
    }

    private Vector3 BaseScale =>
        scaleOwner != null ? scaleOwner.DefaultScale : target.localScale;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        Animate(BaseScale * hoverScale, Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        Animate(BaseScale, Ease.OutSine);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Animate(BaseScale * pressScale, Ease.OutSine);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Animate(isHovering ? BaseScale * hoverScale : BaseScale, Ease.OutBack);
    }

    private void Animate(Vector3 scale, Ease ease)
    {
        tween?.Kill();
        tween = target.DOScale(scale, duration).SetEase(ease);
    }
}