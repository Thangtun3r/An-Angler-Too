using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CollectionToggleAnim : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    [Header("Offsets")]
    [SerializeField] private float toggleOffsetY = 300f;
    [SerializeField] private float hoverOffsetY = 20f;

    [Header("Tween Settings")]
    [SerializeField] private float hoverDuration = 0.15f;
    [SerializeField] private float toggleDuration = 0.25f;

    [SerializeField] private Ease hoverEase = Ease.OutBack;
    [SerializeField] private Ease toggleOnEase = Ease.OutCubic;
    [SerializeField] private Ease toggleOffEase = Ease.OutExpo;

    private RectTransform rect;
    private float baseY;

    private float currentHoverOffset;
    private float currentToggleOffset;

    private bool isToggled;
    private Tween activeTween;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        baseY = rect.anchoredPosition.y;
    }

    void OnEnable()
    {
        PlayerInventory.OnInventoryClosed += ForceToggleOff;
    }

    void OnDisable()
    {
        PlayerInventory.OnInventoryClosed -= ForceToggleOff;
    }

    void Update()
    {
        if (!isToggled) return;

        if (Input.GetMouseButtonDown(0) && !IsPointerInside())
        {
            ToggleOff();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isToggled) return;

        currentHoverOffset = hoverOffsetY;
        MoveTo(baseY + currentToggleOffset + currentHoverOffset, hoverDuration, hoverEase);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isToggled) return;

        currentHoverOffset = 0f;
        MoveTo(baseY + currentToggleOffset, hoverDuration, hoverEase);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isToggled)
            ToggleOn();
    }

    private void ToggleOn()
    {
        isToggled = true;
        currentHoverOffset = 0f;
        currentToggleOffset = toggleOffsetY;

        MoveTo(baseY + currentToggleOffset, toggleDuration, toggleOnEase);
    }

    private void ToggleOff()
    {
        isToggled = false;
        currentToggleOffset = 0f;

        MoveTo(baseY, toggleDuration, toggleOffEase);
    }

    public void ForceToggleOff()
    {
        if (!isToggled) return;
        ToggleOff();
    }

    private bool IsPointerInside()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : canvas.worldCamera;

        return RectTransformUtility.RectangleContainsScreenPoint(
            rect,
            Input.mousePosition,
            cam
        );
    }

    private void MoveTo(float targetY, float duration, Ease ease)
    {
        activeTween?.Kill();
        activeTween = rect
            .DOAnchorPosY(targetY, duration)
            .SetEase(ease);
    }
}
