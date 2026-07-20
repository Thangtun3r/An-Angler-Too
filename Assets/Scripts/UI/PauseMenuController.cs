using System;
using FMOD.Studio;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class PauseMenuController : MonoBehaviour
{
    public static event Action<bool> OnPauseStateChanged;

    [Header("UI")]
    [SerializeField] private GameObject contentRoot;
    [SerializeField] private Slider mouseSensitivitySlider;
    [SerializeField] private Slider masterAudioSlider;
    [SerializeField] private Button reloadGameButton;
    [SerializeField] private Button exitGameButton;
    [SerializeField] private bool createMissingActionButtons = true;

    [Header("Player")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private float minMouseSensitivity = 20f;
    [SerializeField] private float maxMouseSensitivity = 250f;

    [Header("Pause")]
    [SerializeField] private KeyCode toggleKey = KeyCode.Escape;
    [SerializeField] private bool pauseTimeScale = true;
    [SerializeField, Range(0f, 1f)] private float defaultMasterVolume = 1f;

    private const string CursorUnlockReason = "PauseMenu";

    private Bus masterBus;
    private bool isPaused;
    private float previousTimeScale = 1f;

    private void Awake()
    {
        ResolveReferences();
        EnsureActionButtons();
        masterBus = RuntimeManager.GetBus("bus:/");
        InitializeSliders();
        SetPaused(false, false, true);
    }

    private void OnEnable()
    {
        if (mouseSensitivitySlider != null)
            mouseSensitivitySlider.onValueChanged.AddListener(HandleMouseSensitivityChanged);

        if (masterAudioSlider != null)
            masterAudioSlider.onValueChanged.AddListener(HandleMasterAudioChanged);

        if (reloadGameButton != null)
            reloadGameButton.onClick.AddListener(ReloadGame);

        if (exitGameButton != null)
            exitGameButton.onClick.AddListener(ExitGame);
    }

    private void OnDisable()
    {
        if (mouseSensitivitySlider != null)
            mouseSensitivitySlider.onValueChanged.RemoveListener(HandleMouseSensitivityChanged);

        if (masterAudioSlider != null)
            masterAudioSlider.onValueChanged.RemoveListener(HandleMasterAudioChanged);

        if (reloadGameButton != null)
            reloadGameButton.onClick.RemoveListener(ReloadGame);

        if (exitGameButton != null)
            exitGameButton.onClick.RemoveListener(ExitGame);

        if (isPaused)
            SetPaused(false, true, true);
    }

    private void Update()
    {
        if (!Input.GetKeyDown(toggleKey))
            return;

        if (!isPaused && CursorLockManager.IsUnlockedBySomeone)
            return;

        SetPaused(!isPaused, true, false);
    }

    public void Resume()
    {
        SetPaused(false, true, false);
    }

    public void TogglePause()
    {
        SetPaused(!isPaused, true, false);
    }

    public void ReloadGame()
    {
        SetPaused(false, true, true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        SetPaused(false, true, true);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void ResolveReferences()
    {
        if (contentRoot == null && transform.childCount > 0)
            contentRoot = transform.GetChild(0).gameObject;

        if (playerMovement == null)
            playerMovement = FindObjectOfType<PlayerMovement>();
    }

    private void EnsureActionButtons()
    {
        if (contentRoot == null)
            return;

        if (reloadGameButton == null)
            reloadGameButton = FindButton("Reload Game Button");

        if (exitGameButton == null)
            exitGameButton = FindButton("Exit Game Button");

        if (!createMissingActionButtons)
            return;

        if (reloadGameButton == null)
            reloadGameButton = CreateActionButton("Reload Game Button", "Reload", new Vector2(-110f, -150f));

        if (exitGameButton == null)
            exitGameButton = CreateActionButton("Exit Game Button", "Exit", new Vector2(110f, -150f));
    }

    private Button FindButton(string objectName)
    {
        Transform buttonTransform = contentRoot.transform.Find(objectName);
        return buttonTransform != null ? buttonTransform.GetComponent<Button>() : null;
    }

    private Button CreateActionButton(string objectName, string label, Vector2 anchoredPosition)
    {
        GameObject buttonObject = new GameObject(
            objectName,
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image),
            typeof(Button)
        );

        buttonObject.transform.SetParent(contentRoot.transform, false);

        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.pivot = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = anchoredPosition;
        buttonRect.sizeDelta = new Vector2(180f, 48f);

        Image buttonImage = buttonObject.GetComponent<Image>();
        buttonImage.color = new Color(0f, 0f, 0f, 0.55f);

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = buttonImage;

        GameObject labelObject = new GameObject(
            "Label",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(TextMeshProUGUI)
        );

        labelObject.transform.SetParent(buttonObject.transform, false);

        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        TextMeshProUGUI labelText = labelObject.GetComponent<TextMeshProUGUI>();
        labelText.text = label;
        labelText.alignment = TextAlignmentOptions.Center;
        labelText.color = Color.white;
        labelText.fontSize = 24f;
        labelText.raycastTarget = false;

        return button;
    }

    private void InitializeSliders()
    {
        if (mouseSensitivitySlider != null && playerMovement != null)
        {
            float value = Mathf.InverseLerp(minMouseSensitivity, maxMouseSensitivity, playerMovement.mouseSensitivity);
            mouseSensitivitySlider.SetValueWithoutNotify(value);
        }

        if (masterAudioSlider != null)
            masterAudioSlider.SetValueWithoutNotify(defaultMasterVolume);

        HandleMouseSensitivityChanged(mouseSensitivitySlider != null ? mouseSensitivitySlider.value : 0f);
        HandleMasterAudioChanged(masterAudioSlider != null ? masterAudioSlider.value : defaultMasterVolume);
    }

    private void SetPaused(bool paused, bool notify, bool force)
    {
        if (!force && isPaused == paused)
            return;

        isPaused = paused;

        if (contentRoot != null)
            contentRoot.SetActive(isPaused);

        if (pauseTimeScale)
        {
            if (isPaused)
            {
                previousTimeScale = Time.timeScale;
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = previousTimeScale;
            }
        }

        if (isPaused)
            CursorLockManager.RequestUnlock(CursorUnlockReason);
        else
            CursorLockManager.ReleaseUnlock(CursorUnlockReason);

        if (notify)
            OnPauseStateChanged?.Invoke(isPaused);
    }

    private void HandleMouseSensitivityChanged(float value)
    {
        if (playerMovement == null)
            return;

        playerMovement.mouseSensitivity = Mathf.Lerp(minMouseSensitivity, maxMouseSensitivity, value);
    }

    private void HandleMasterAudioChanged(float value)
    {
        if (masterBus.isValid())
            masterBus.setVolume(value);
    }
}
