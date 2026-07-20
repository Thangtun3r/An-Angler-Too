using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Yarn.Markup;
using Yarn.Unity;
using TMPro;

[DisallowMultipleComponent]
public sealed class DialoguePanelClickAdvance : MonoBehaviour, IPointerClickHandler, IActionMarkupHandler
{
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] private DialoguePresenterBase linePresenter;
    [SerializeField] private Button continueButton;
    [SerializeField] private CanvasGroup optionsCanvasGroup;
    [SerializeField] private bool ignoreClicksWhenOptionsAreVisible = true;

    private bool isLineActive;
    private bool isLineFullyVisible;

    private void Awake()
    {
        if (dialogueRunner == null)
            dialogueRunner = FindFirstObjectByType<DialogueRunner>(FindObjectsInactive.Include);

        if (linePresenter == null)
            linePresenter = FindFirstObjectByType<LinePresenter>(FindObjectsInactive.Include);

        if (continueButton == null)
            continueButton = GetComponentInChildren<Button>(true);
    }

    private void OnEnable()
    {
        RegisterTypewriterHandler();
        RegisterContinueButton();
    }

    private void Start()
    {
        RegisterTypewriterHandler();
        RegisterContinueButton();
        SetContinueButtonEnabled(false);
    }

    private void OnDisable()
    {
        if (linePresenter?.Typewriter != null)
            linePresenter.Typewriter.ActionMarkupHandlers.Remove(this);

        if (continueButton != null)
            continueButton.onClick.RemoveListener(TryAdvanceDialogue);

        isLineActive = false;
        isLineFullyVisible = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        TryAdvanceDialogue();
    }

    public void TryAdvanceDialogue()
    {
        if (dialogueRunner == null || !dialogueRunner.IsDialogueRunning)
            return;

        if (ignoreClicksWhenOptionsAreVisible && OptionsAreVisible())
            return;

        if (!isLineActive)
            return;

        if (isLineFullyVisible)
            dialogueRunner.RequestNextLine();
        else
            dialogueRunner.RequestHurryUpLine();
    }

    public void OnPrepareForLine(MarkupParseResult line, TMP_Text text)
    {
        isLineActive = true;
        isLineFullyVisible = false;
        SetContinueButtonEnabled(true);
    }

    public void OnLineDisplayBegin(MarkupParseResult line, TMP_Text text)
    {
        isLineActive = true;
        isLineFullyVisible = false;
        SetContinueButtonEnabled(true);
    }

    public YarnTask OnCharacterWillAppear(int currentCharacterIndex, MarkupParseResult line, CancellationToken cancellationToken)
    {
        return YarnTask.CompletedTask;
    }

    public void OnLineDisplayComplete()
    {
        isLineFullyVisible = true;
    }

    public void OnLineWillDismiss()
    {
        isLineActive = false;
        isLineFullyVisible = false;
        SetContinueButtonEnabled(false);
    }

    private void RegisterTypewriterHandler()
    {
        if (linePresenter?.Typewriter == null)
            return;

        if (!linePresenter.Typewriter.ActionMarkupHandlers.Contains(this))
            linePresenter.Typewriter.ActionMarkupHandlers.Add(this);
    }

    private void RegisterContinueButton()
    {
        if (continueButton == null)
            return;

        continueButton.onClick.RemoveListener(TryAdvanceDialogue);
        continueButton.onClick.AddListener(TryAdvanceDialogue);
    }

    private void SetContinueButtonEnabled(bool enabled)
    {
        if (continueButton == null)
            return;

        continueButton.interactable = enabled;
        continueButton.enabled = enabled;
    }

    private bool OptionsAreVisible()
    {
        return optionsCanvasGroup != null
            && optionsCanvasGroup.gameObject.activeInHierarchy
            && optionsCanvasGroup.alpha > 0.01f
            && optionsCanvasGroup.blocksRaycasts;
    }
}
