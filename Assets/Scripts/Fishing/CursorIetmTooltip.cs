using UnityEngine;
using TMPro;
using DG.Tweening;

public class CursorItemTooltip : MonoBehaviour
{
    [Header("References")]
    public RectTransform root;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;

    [Header("Show Tween")]
    public float showDuration = 0.15f;
    public Ease showEase = Ease.OutBack;

    private Vector3 defaultScale;
    private bool isVisible;

    void Awake()
    {
        if (root == null)
            root = transform as RectTransform;

        // ✅ CACHE AUTHORED SCALE (this is the key)
        defaultScale = root.localScale;

        // Start hidden
        root.localScale = Vector3.zero;
        gameObject.SetActive(false);
        isVisible = false;

        // Safety: never block input
        var cg = GetComponent<CanvasGroup>();
        if (cg == null) cg = gameObject.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
        cg.interactable = false;
    }

    public void Show(ItemSO item)
    {
        if (item == null)
            return;

        nameText.text = item.item_name;
        descText.text = item.ItemDescription;

        // 🔒 HARD RESET (prevents scale stacking)
        root.DOKill();
        root.localScale = Vector3.zero;

        gameObject.SetActive(true);
        isVisible = true;

        // ✅ Tween to AUTHORED scale
        root.DOScale(defaultScale, showDuration)
            .SetEase(showEase)
            .SetUpdate(true);
    }

    public void Hide()
    {
        if (!isVisible)
            return;

        isVisible = false;

        // ❌ NO HIDE TWEEN — IMMEDIATE
        root.DOKill();
        root.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }
}