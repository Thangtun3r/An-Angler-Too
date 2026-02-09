using UnityEngine;
using DG.Tweening;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class UIScaleInOnEnable : MonoBehaviour
{
    [SerializeField] private float duration = 0.25f;
    [SerializeField] private Ease ease = Ease.OutBack;

    public Vector3 DefaultScale { get; private set; }

    private RectTransform rect;
    private Tween tween;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        DefaultScale = rect.localScale; // capture once, forever
    }

    private void OnEnable()
    {
        tween?.Kill();
        rect.localScale = Vector3.zero;
        StartCoroutine(PlayNextFrame());
    }

    private IEnumerator PlayNextFrame()
    {
        yield return null;

        tween = rect
            .DOScale(DefaultScale, duration)
            .SetEase(ease)
            .SetUpdate(true);
    }

    private void OnDisable()
    {
        tween?.Kill();
    }
}