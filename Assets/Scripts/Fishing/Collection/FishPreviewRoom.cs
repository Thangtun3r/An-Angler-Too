using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FishPreviewRoom : MonoBehaviour
{
    public static FishPreviewRoom Instance { get; private set; }

    [Header("Spawn Holder")]
    public Transform holder;

    [Header("Source of Truth")]
    public CollectionUIBuilder uiBuilder; // drag builder or it will FindObjectOfType

    [Header("Preview Layer Name")]
    public string previewLayerName = "FishPreview";

    [Header("Holder Pop (on change preview)")]
    public float popDuration = 0.18f;
    public Ease popEase = Ease.OutBack;
    public float popFromScale = 0f;

    [Header("Holder Rotation Loop")]
    public bool rotate = true;
    public Vector3 rotationPerLoop = new Vector3(0f, 360f, 0f);
    public float rotationDuration = 6f;          // seconds per 360
    public RotateMode rotateMode = RotateMode.FastBeyond360;
    public Ease rotationEase = Ease.Linear;

    private readonly Dictionary<string, GameObject> previewMap = new Dictionary<string, GameObject>();
    private GameObject currentPreview;

    private Vector3 holderDefaultScale;
    private Tween rotateTween;
    private Tween popTween;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (holder != null)
            holderDefaultScale = holder.localScale;
    }

    private void Start()
    {
        if (holder == null)
        {
            Debug.LogError("FishPreviewRoom missing holder.");
            return;
        }

        // cache default scale in case holder assigned after Awake
        holderDefaultScale = holder.localScale;

        if (uiBuilder == null)
            uiBuilder = FindObjectOfType<CollectionUIBuilder>();

        if (uiBuilder == null || uiBuilder.allFish == null || uiBuilder.allFish.Count == 0)
        {
            Debug.LogError("FishPreviewRoom: Cannot find CollectionUIBuilder or its allFish is empty.");
            return;
        }

        PrewarmFrom(uiBuilder.allFish);

        Clear();          // hides any preview
        StartRotation();  // seamless loop
    }

    private void PrewarmFrom(List<ItemSO> fishList)
    {
        previewMap.Clear();

        int previewLayer = LayerMask.NameToLayer(previewLayerName);
        if (previewLayer == -1)
            Debug.LogWarning($"Layer '{previewLayerName}' not found. Previews will keep default layers.");

        foreach (var fish in fishList)
        {
            if (fish == null || fish.item_prefab == null) continue;
            if (string.IsNullOrEmpty(fish.itemID)) continue;

            if (previewMap.ContainsKey(fish.itemID))
            {
                Debug.LogWarning($"Duplicate fish itemID: {fish.itemID}");
                continue;
            }

            GameObject go = Instantiate(fish.item_prefab, holder);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.SetActive(false);

            if (previewLayer != -1)
                SetLayerRecursively(go, previewLayer);

            previewMap.Add(fish.itemID, go);
        }
    }

    public void Show(ItemSO item)
    {
        if (item == null || string.IsNullOrEmpty(item.itemID)) return;

        if (currentPreview != null)
            currentPreview.SetActive(false);

        if (!previewMap.TryGetValue(item.itemID, out GameObject preview) || preview == null)
        {
            currentPreview = null;
            return;
        }

        currentPreview = preview;
        currentPreview.SetActive(true);

        PopHolder();
    }

    public void Clear()
    {
        if (currentPreview != null)
        {
            currentPreview.SetActive(false);
            currentPreview = null;
        }
    }

    private void PopHolder()
    {
        if (holder == null) return;
        
        popTween?.Kill();
        holder.localScale = holderDefaultScale * popFromScale;
        popTween = holder.DOScale(holderDefaultScale, popDuration)
                         .SetEase(popEase)
                         .SetUpdate(true); 
    }

    private void StartRotation()
    {
        if (!rotate || holder == null) return;

        rotateTween?.Kill();
        holder.localRotation = Quaternion.identity;

        rotateTween = holder.DOLocalRotate(rotationPerLoop, rotationDuration, rotateMode)
                            .SetEase(rotationEase)
                            .SetLoops(-1, LoopType.Restart);
    }

    private void OnDisable()
    {
        rotateTween?.Kill();
        popTween?.Kill();
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, layer);
    }
}
