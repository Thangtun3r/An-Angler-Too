using UnityEngine;
using DG.Tweening;

public class NotEnoughFihFeedback : MonoBehaviour
{
    [SerializeField] private RectTransform rect;
    [SerializeField] private CanvasGroup canvas;

    [Header("Animation")]
    [SerializeField] private float fadeInAlpha = 0.7f;
    [SerializeField] private float fadeInTime = 0.15f;
    [SerializeField] private float fadeOutTime = 0.2f;
    [SerializeField] private float shakeDuration = 0.4f;
    [SerializeField] private float shakeStrength = 12f;
    [SerializeField] private int shakeVibrato = 12;

    private void Awake()
    {
        if (canvas != null)
            canvas.alpha = 0f;
    }

    public void Play()
    {
        Debug.Log("Not enough fih feedback!");

        if (rect == null || canvas == null)
            return;

        rect.DOKill();
        canvas.DOKill();

        canvas.alpha = 0f;

        canvas
            .DOFade(fadeInAlpha, fadeInTime)
            .SetEase(Ease.OutSine);

        rect
            .DOShakeAnchorPos(
                shakeDuration,
                new Vector2(shakeStrength, 0),
                shakeVibrato,
                90,
                false,
                true
            );

        canvas
            .DOFade(0f, fadeOutTime)
            .SetDelay(shakeDuration)
            .SetEase(Ease.InSine);
    }
}