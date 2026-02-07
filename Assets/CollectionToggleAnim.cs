using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CollectionToggleAnim : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    [Header("Positions")]
    [SerializeField] private float toggledY = 300f;
    [SerializeField] private float hoverOffsetY = 20f;

    [Header("Tween Settings")]
    [SerializeField] private float hoverDuration = 0.15f;
    [SerializeField] private float toggleDuration = 0.25f;

    [SerializeField] private Ease hoverEase = Ease.OutBack;
    [SerializeField] private Ease toggleOnEase = Ease.OutCubic;
    [SerializeField] private Ease toggleOffEase = Ease.OutExpo;

    private RectTransform rect;
    private Vector2 originalPos;
    private bool isToggled;

    private Tween activeTween;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        originalPos = rect.anchoredPosition;
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

        if (Input.GetMouseButtonDown(0))
        {
            if (!IsPointerInside())
            {
                ToggleOff();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isToggled) return;
        MoveY(originalPos.y + hoverOffsetY, hoverDuration, hoverEase);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isToggled) return;
        MoveY(originalPos.y, hoverDuration, hoverEase);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isToggled)
            ToggleOn();
    }

    private void ToggleOn()
    {
        isToggled = true;
        MoveY(toggledY, toggleDuration, toggleOnEase);
    }

    private void ToggleOff()
    {
        isToggled = false;
        MoveY(originalPos.y, toggleDuration, toggleOffEase);
    }

    public void ForceToggleOff()
    {
        if (!isToggled) return;
        ToggleOff();
    }

    private bool IsPointerInside()
    {
        Camera cam = null;

        if (GetComponentInParent<Canvas>().renderMode != RenderMode.ScreenSpaceOverlay)
            cam = GetComponentInParent<Canvas>().worldCamera;

        return RectTransformUtility.RectangleContainsScreenPoint(
            rect,
            Input.mousePosition,
            cam
        );
    }

    private void MoveY(float y, float duration, Ease ease)
    {
        activeTween?.Kill();
        activeTween = rect.DOAnchorPosY(y, duration).SetEase(ease);
    }
}
