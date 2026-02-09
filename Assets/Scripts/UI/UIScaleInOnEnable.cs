using UnityEngine;
using DG.Tweening;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class UIScaleInOnEnable : MonoBehaviour
{
    [SerializeField] private float duration = 0.25f;
    [SerializeField] private Ease ease = Ease.OutBack;

    private RectTransform rect;
    private Vector3 defaultScale;
    private Tween tween;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        defaultScale = rect.localScale;
    }

    private void OnEnable()
    {
        if (rect == null) return;

        tween?.Kill();

        // Force invisible BEFORE layout & render
        rect.localScale = Vector3.zero;

        // Wait 1 frame so Canvas + Layout settle
        StartCoroutine(PlayScaleInNextFrame());
    }

    private IEnumerator PlayScaleInNextFrame()
    {
        yield return null; // ← critical line

        tween = rect
            .DOScale(defaultScale, duration)
            .SetEase(ease)
            .SetUpdate(true); // works even if timescale = 0
    }

    private void OnDisable()
    {
        tween?.Kill();
    }
}