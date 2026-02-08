using UnityEngine;
using DG.Tweening;

public class StoreDisplayerFollowMouse : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform anchor;      // follows mouse
    [SerializeField] private RectTransform displayer;   // child (actual panel)
    [SerializeField] private Canvas canvas;

    [Header("Offsets")]
    [SerializeField] private Vector2 offsetRight = new Vector2(40, -40);
    [SerializeField] private Vector2 offsetLeft  = new Vector2(-40, -40);

    [Header("Tween")]
    [SerializeField] private float tweenDuration = 0.15f;
    [SerializeField] private Ease easeIn = Ease.InSine;
    [SerializeField] private Ease easeOut = Ease.OutBack;

    private Tween tween;
    private RectTransform canvasRect;

    private void Awake()
    {
        if (anchor == null)
            anchor = transform as RectTransform;

        canvasRect = canvas.transform as RectTransform;

        displayer.localScale = Vector3.zero;
    }

    private void Update()
    {
        FollowMouse();
        UpdateOffsetBasedOnScreen();
    }

    // ---------------- Mouse Follow ----------------

    void FollowMouse()
    {
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            canvas.worldCamera,
            out localPos
        );

        anchor.localPosition = localPos;
    }

    // ---------------- Screen Bounds Logic ----------------

    void UpdateOffsetBasedOnScreen()
    {
        Vector3[] corners = new Vector3[4];
        displayer.GetWorldCorners(corners);

        bool overflowRight = false;
        bool overflowLeft = false;

        foreach (var c in corners)
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(
                canvas.worldCamera,
                c
            );

            if (screenPoint.x > Screen.width)
                overflowRight = true;
            if (screenPoint.x < 0)
                overflowLeft = true;
        }

        if (overflowRight)
            displayer.anchoredPosition = offsetLeft;
        else
            displayer.anchoredPosition = offsetRight;
    }

    // ---------------- Public Controls ----------------

    public void Show()
    {
        tween?.Kill();
        tween = displayer.DOScale(1f, tweenDuration)
            .SetEase(easeOut);
    }

    public void Hide()
    {
        tween?.Kill();
        tween = displayer.DOScale(0f, tweenDuration)
            .SetEase(easeIn);
    }
}
