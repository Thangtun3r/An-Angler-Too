using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CursorFollower : MonoBehaviour
{
    public static CursorFollower Instance;

    [Header("Images")]
    public Image fishIconImage;
    public Image selectionCrosshair;

    private RectTransform crosshairRect;
    private RectTransform fishRect;
    private CanvasGroup canvasGroup;

    private Vector3 crosshairOriginalScale;
    private Vector3 fishOriginalScale;

    private bool followMouse;

    void Awake()
    {
        Instance = this;

        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        fishIconImage.raycastTarget = false;
        selectionCrosshair.raycastTarget = false;

        crosshairRect = selectionCrosshair.rectTransform;
        fishRect = fishIconImage.rectTransform;

        crosshairOriginalScale = crosshairRect.localScale;
        fishOriginalScale = fishRect.localScale;

        selectionCrosshair.enabled = false;
        fishIconImage.enabled = false;

        crosshairRect.localScale = Vector3.zero;
        fishRect.localScale = fishOriginalScale;
    }

    void Update()
    {
        if (!followMouse) return;

        Vector3 mousePos = Input.mousePosition;
        crosshairRect.position = mousePos;
        fishRect.position = mousePos;
    }

    public void SnapTo(RectTransform target)
    {
        followMouse = false;

        selectionCrosshair.enabled = true;
        fishIconImage.enabled = false;

        crosshairRect.DOKill();
        fishRect.DOKill();

        crosshairRect.rotation = Quaternion.identity;
        crosshairRect.position = target.position;
        fishRect.position = target.position;

        crosshairRect.localScale = Vector3.zero;
        crosshairRect
            .DOScale(crosshairOriginalScale, 0.15f)
            .SetEase(Ease.OutBack);
    }
    

    public void Select(Sprite fishSprite)
    {
        followMouse = true;

        fishIconImage.sprite = fishSprite;
        fishIconImage.enabled = true;
        fishIconImage.color = Color.white;

        selectionCrosshair.enabled = true;

        fishRect.DOKill();
        fishRect.localScale = Vector3.zero;
        fishRect
            .DOScale(fishOriginalScale * 1.2f, 0.15f)
            .SetEase(Ease.OutBack);

        crosshairRect.DOKill();
        crosshairRect
            .DOScale(crosshairOriginalScale * 1.2f, 0.15f)
            .SetEase(Ease.OutBack);

        crosshairRect
            .DORotate(new Vector3(0, 0, -45f), 0.15f);
    }
    
    public void Clear()
    {
        followMouse = false;

        crosshairRect.DOKill();
        fishRect.DOKill();

        crosshairRect
            .DOScale(Vector3.zero, 0.12f)
            .SetEase(Ease.InBack);

        if (fishIconImage.enabled)
        {
            fishRect
                .DOScale(Vector3.zero, 0.12f)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    fishIconImage.enabled = false;
                    fishRect.localScale = fishOriginalScale;
                    selectionCrosshair.enabled = false;
                });
        }
        else
        {
            selectionCrosshair.enabled = false;
        }
    }
}
