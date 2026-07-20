using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[DisallowMultipleComponent]
public sealed class PixelatedRenderTextureScaler : MonoBehaviour
{
    [SerializeField] private RenderTexture sourceRenderTexture;
    [SerializeField, Min(1)] private int screenPixelsPerTexel = 4;
    [SerializeField, Min(16)] private int minimumWidth = 16;
    [SerializeField, Min(16)] private int minimumHeight = 16;
    [SerializeField] private bool previewInEditMode = true;
    [SerializeField] private RawImage outputImage;
    [SerializeField] private bool bindSceneCamerasUsingSourceTexture = true;

    private readonly List<Camera> reboundCameras = new List<Camera>();
    private RenderTexture runtimeRenderTexture;
    private int currentWidth = -1;
    private int currentHeight = -1;

    private void OnEnable()
    {
        ResolveReferences();
        UpdateRenderTextureBinding(true);
    }

    private void LateUpdate()
    {
        UpdateRenderTextureBinding(false);
    }

    private void OnValidate()
    {
        screenPixelsPerTexel = Mathf.Max(1, screenPixelsPerTexel);
        minimumWidth = Mathf.Max(16, minimumWidth);
        minimumHeight = Mathf.Max(16, minimumHeight);
        ResolveReferences();
    }

    private void OnDisable()
    {
        RestoreBindings();
        ReleaseRuntimeRenderTexture();
    }

    private void ResolveReferences()
    {
        if (outputImage == null)
            outputImage = GetComponentInChildren<RawImage>(true);

        if (sourceRenderTexture == null && outputImage != null)
            sourceRenderTexture = outputImage.texture as RenderTexture;
    }

    private void UpdateRenderTextureBinding(bool force)
    {
        if (!ShouldUpdate())
            return;

        ResolveReferences();

        if (sourceRenderTexture == null || outputImage == null)
            return;

        int screenWidth;
        int screenHeight;
        GetOutputSize(out screenWidth, out screenHeight);

        int targetWidth = Mathf.Max(minimumWidth, Mathf.CeilToInt(screenWidth / (float)screenPixelsPerTexel));
        int targetHeight = Mathf.Max(minimumHeight, Mathf.CeilToInt(screenHeight / (float)screenPixelsPerTexel));

        RenderTexture previousRuntimeTexture = runtimeRenderTexture;
        bool resized = EnsureRuntimeRenderTexture(targetWidth, targetHeight, force);
        if (resized)
        {
            RebindCameras(previousRuntimeTexture);
            if (outputImage.texture != runtimeRenderTexture)
                outputImage.texture = runtimeRenderTexture;

            ReleaseRenderTexture(previousRuntimeTexture);
        }

        if (outputImage.texture != runtimeRenderTexture)
            outputImage.texture = runtimeRenderTexture;

        if (bindSceneCamerasUsingSourceTexture && !resized)
            RebindCameras(null);
    }

    private bool ShouldUpdate()
    {
        if (!isActiveAndEnabled)
            return false;

        if (!Application.isPlaying && !previewInEditMode)
            return false;

        return gameObject.scene.IsValid() && gameObject.scene.isLoaded;
    }

    private void GetOutputSize(out int width, out int height)
    {
        RectTransform rectTransform = outputImage.rectTransform;
        Rect rect = rectTransform.rect;

        width = Mathf.RoundToInt(rect.width);
        height = Mathf.RoundToInt(rect.height);

        if (width <= 0)
            width = Screen.width;

        if (height <= 0)
            height = Screen.height;
    }

    private bool EnsureRuntimeRenderTexture(int width, int height, bool force)
    {
        if (!force && runtimeRenderTexture != null && currentWidth == width && currentHeight == height)
            return false;

        RenderTextureDescriptor descriptor = sourceRenderTexture.descriptor;
        descriptor.width = width;
        descriptor.height = height;
        descriptor.msaaSamples = 1;
        descriptor.useMipMap = false;
        descriptor.autoGenerateMips = false;

        runtimeRenderTexture = new RenderTexture(descriptor)
        {
            name = sourceRenderTexture.name + "_ScaledRuntime",
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp,
            hideFlags = HideFlags.HideAndDontSave
        };
        runtimeRenderTexture.Create();

        currentWidth = width;
        currentHeight = height;

        return true;
    }

    private void RebindCameras(RenderTexture previousRuntimeTexture)
    {
        if (!bindSceneCamerasUsingSourceTexture || runtimeRenderTexture == null)
            return;

        Camera[] cameras = FindObjectsOfType<Camera>(true);
        foreach (Camera sceneCamera in cameras)
        {
            if (sceneCamera.targetTexture != sourceRenderTexture && sceneCamera.targetTexture != previousRuntimeTexture)
                continue;

            sceneCamera.targetTexture = runtimeRenderTexture;

            if (!reboundCameras.Contains(sceneCamera))
                reboundCameras.Add(sceneCamera);
        }
    }

    private void RestoreBindings()
    {
        if (outputImage != null && outputImage.texture == runtimeRenderTexture)
            outputImage.texture = sourceRenderTexture;

        for (int i = reboundCameras.Count - 1; i >= 0; i--)
        {
            Camera reboundCamera = reboundCameras[i];
            if (reboundCamera != null && reboundCamera.targetTexture == runtimeRenderTexture)
                reboundCamera.targetTexture = sourceRenderTexture;
        }

        reboundCameras.Clear();
    }

    private void ReleaseRuntimeRenderTexture()
    {
        if (runtimeRenderTexture == null)
            return;

        ReleaseRenderTexture(runtimeRenderTexture);
        runtimeRenderTexture = null;
        currentWidth = -1;
        currentHeight = -1;
    }

    private void ReleaseRenderTexture(RenderTexture renderTexture)
    {
        if (renderTexture == null)
            return;

        renderTexture.Release();

        if (Application.isPlaying)
            Destroy(renderTexture);
        else
            DestroyImmediate(renderTexture);
    }
}
