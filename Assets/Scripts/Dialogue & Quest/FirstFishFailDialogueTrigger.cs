using System.Collections;
using UnityEngine;
using Yarn.Unity;

[DisallowMultipleComponent]
public sealed class FirstFishFailDialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] private string node = "FishFailedTutorial";

    private bool hasShown;
    private Coroutine pendingDialogue;

    private void Awake()
    {
        if (dialogueRunner == null)
            dialogueRunner = FindFirstObjectByType<DialogueRunner>(FindObjectsInactive.Include);
    }

    private void OnEnable()
    {
        FishingCast.OnFishCatchFailed += HandleFishCatchFailed;
    }

    private void OnDisable()
    {
        FishingCast.OnFishCatchFailed -= HandleFishCatchFailed;

        if (pendingDialogue != null)
            StopCoroutine(pendingDialogue);

        pendingDialogue = null;
    }

    private void HandleFishCatchFailed()
    {
        if (hasShown || pendingDialogue != null)
            return;

        pendingDialogue = StartCoroutine(StartDialogueAfterDelay());
    }

    private IEnumerator StartDialogueAfterDelay()
    {
        yield return null;

        pendingDialogue = null;

        if (hasShown || dialogueRunner == null || dialogueRunner.IsDialogueRunning)
            yield break;

        hasShown = true;
        dialogueRunner.StartDialogue(node);
    }
}
