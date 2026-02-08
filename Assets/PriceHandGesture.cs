using UnityEngine;
using DG.Tweening;

public class PriceHandGesture : MonoBehaviour
{
    [Header("Renderer")]
    [SerializeField] private SpriteRenderer handRenderer;

    [Header("Animated Object (Child)")]
    [SerializeField] private Transform handVisual;

    [Header("Gesture Sprites (index = amount - 1)")]
    [SerializeField] private Sprite[] gestureSprites;

    [Header("Positions (Local Y)")]
    [SerializeField] private float shownY;
    [SerializeField] private float hiddenY;

    [Header("Tween Settings")]
    [SerializeField] private float tweenDuration = 0.2f;
    [SerializeField] private Ease tweenEase = Ease.InOutSine;

    private Tween currentTween;

    private void Awake()
    {
        if (handVisual == null)
            handVisual = transform;

        // Auto-capture shown position if not manually set
        if (Mathf.Approximately(shownY, 0f))
            shownY = handVisual.localPosition.y;
    }

    private void OnEnable()
    {
        // CLICK-ONLY signal from UIStoreSlot
        UIStoreSlot.OnStoreSlotClickedForGesture += OnSlotClicked;
    }

    private void OnDisable()
    {
        UIStoreSlot.OnStoreSlotClickedForGesture -= OnSlotClicked;
    }

    // ---------------- Event ----------------

    private void OnSlotClicked(UIStoreSlot slot)
    {
        int amount = GetPriceAmount(slot);
        AnimateGestureChange(amount);
    }

    // ---------------- Price Logic ----------------

    int GetPriceAmount(UIStoreSlot slot)
    {
        if (slot.acceptAnyFish)
            return slot.anyFishAmount;

        int total = 0;
        foreach (var cost in slot.costs)
            total += cost.amount;

        return total;
    }

    // ---------------- Animation ----------------

    void AnimateGestureChange(int amount)
    {
        if (handRenderer == null || handVisual == null || gestureSprites.Length == 0)
            return;

        int index = Mathf.Clamp(amount - 1, 0, gestureSprites.Length - 1);

        currentTween?.Kill();

        // Hide hand
        currentTween = handVisual
            .DOLocalMoveY(hiddenY, tweenDuration)
            .SetEase(tweenEase)
            .OnComplete(() =>
            {
                // Swap sprite while hidden
                handRenderer.sprite = gestureSprites[index];
                handRenderer.enabled = true;

                // Show hand
                handVisual
                    .DOLocalMoveY(shownY, tweenDuration)
                    .SetEase(tweenEase);
            });
    }

    // ---------------- Public ----------------

    public void HideInstant()
    {
        currentTween?.Kill();

        if (handRenderer != null)
            handRenderer.enabled = false;

        if (handVisual != null)
        {
            handVisual.localPosition = new Vector3(
                handVisual.localPosition.x,
                hiddenY,
                handVisual.localPosition.z
            );
        }
    }
}
