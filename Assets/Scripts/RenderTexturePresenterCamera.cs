using UnityEngine;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
[DisallowMultipleComponent]
public sealed class RenderTexturePresenterCamera : MonoBehaviour
{
    private const string CameraObjectName = "__DisplayPresenterCamera";

    [SerializeField] private string presenterName = "Display Presenter Camera";
    [SerializeField] private CameraClearFlags clearFlags = CameraClearFlags.SolidColor;
    [SerializeField] private Color clearColor = Color.black;
    [SerializeField] private int targetDisplay = 0;
    [SerializeField] private float depth = -1000f;
    [SerializeField] private bool createInEditMode = true;

    private Camera presenterCamera;

    private void OnEnable()
    {
        EnsurePresenterCamera();
    }

    private void LateUpdate()
    {
        EnsurePresenterCamera();
    }

    private void OnValidate()
    {
        if (presenterCamera != null)
            ApplyPresenterSettings();
    }

    private void OnDisable()
    {
        DestroyPresenterCamera();
    }

    private bool ShouldCreatePresenter()
    {
        if (!isActiveAndEnabled)
            return false;

        if (!Application.isPlaying && !createInEditMode)
            return false;

        return gameObject.scene.IsValid() && gameObject.scene.isLoaded;
    }

    private void EnsurePresenterCamera()
    {
        if (!ShouldCreatePresenter())
            return;

        if (presenterCamera == null)
            presenterCamera = FindPresenterCamera();

        if (presenterCamera == null)
            presenterCamera = CreatePresenterCamera();

        ApplyPresenterSettings();
    }

    private Camera FindPresenterCamera()
    {
        Transform existing = transform.Find(CameraObjectName);
        if (existing != null)
            return existing.GetComponent<Camera>();

        if (!string.IsNullOrWhiteSpace(presenterName))
        {
            existing = transform.Find(presenterName);
            if (existing != null)
                return existing.GetComponent<Camera>();
        }

        return null;
    }

    private Camera CreatePresenterCamera()
    {
        var cameraObject = new GameObject(CameraObjectName);
        cameraObject.hideFlags = HideFlags.HideAndDontSave;
        cameraObject.transform.SetParent(transform, false);

        return cameraObject.AddComponent<Camera>();
    }

    private void ApplyPresenterSettings()
    {
        GameObject cameraObject = presenterCamera.gameObject;
        cameraObject.name = string.IsNullOrWhiteSpace(presenterName)
            ? CameraObjectName
            : presenterName;
        cameraObject.hideFlags = HideFlags.HideAndDontSave;
        cameraObject.SetActive(true);

        presenterCamera.enabled = true;
        presenterCamera.clearFlags = clearFlags;
        presenterCamera.backgroundColor = clearColor;
        presenterCamera.cullingMask = 0;
        presenterCamera.targetTexture = null;
        presenterCamera.targetDisplay = targetDisplay;
        presenterCamera.depth = depth;
        presenterCamera.orthographic = true;
        presenterCamera.allowHDR = false;
        presenterCamera.allowMSAA = false;

        var cameraData = presenterCamera.GetUniversalAdditionalCameraData();
        cameraData.renderType = CameraRenderType.Base;
        cameraData.renderPostProcessing = false;
        cameraData.requiresDepthOption = CameraOverrideOption.Off;
        cameraData.requiresColorOption = CameraOverrideOption.Off;
        cameraData.antialiasing = AntialiasingMode.None;
        cameraData.SetRenderer(0);
    }

    private void DestroyPresenterCamera()
    {
        if (presenterCamera == null)
            return;

        GameObject cameraObject = presenterCamera.gameObject;
        presenterCamera = null;

        if (Application.isPlaying)
            Destroy(cameraObject);
        else
            DestroyImmediate(cameraObject);
    }
}
