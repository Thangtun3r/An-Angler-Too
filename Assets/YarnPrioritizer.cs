using UnityEngine;
using Yarn.Unity;

public class DialogueNodeMouseAndCanvasControl : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueRunner dialogueRunner;

    [Header("Mouse")]
    [SerializeField] private bool unlockMouseOnDialogue = true;

    [Header("Canvas Control")]
    [Tooltip("These canvases will be DISABLED while dialogue nodes are running")]
    [SerializeField] private Canvas[] canvasesToDisable;

    private bool wasMouseLockedBeforeDialogue;

    private void Awake()
    {
        if (dialogueRunner == null)
        {
            dialogueRunner = FindFirstObjectByType<DialogueRunner>();
        }
    }

    private void OnEnable()
    {
        if (dialogueRunner == null) return;

        dialogueRunner.onNodeStart.AddListener(OnNodeStarted);
        dialogueRunner.onNodeComplete.AddListener(OnNodeCompleted);
        dialogueRunner.onDialogueComplete.AddListener(OnDialogueCompleted);
    }

    private void OnDisable()
    {
        if (dialogueRunner == null) return;

        dialogueRunner.onNodeStart.RemoveListener(OnNodeStarted);
        dialogueRunner.onNodeComplete.RemoveListener(OnNodeCompleted);
        dialogueRunner.onDialogueComplete.RemoveListener(OnDialogueCompleted);
    }
    

    private void OnNodeStarted(string nodeName)
    {
        if (unlockMouseOnDialogue)
        {
            StoreAndUnlockMouse();
        }

        SetCanvasesActive(false);
    }

    private void OnNodeCompleted(string nodeName)
    {
    }

    private void OnDialogueCompleted()
    {
        RestoreMouseState();
        SetCanvasesActive(true);
    }

    private void StoreAndUnlockMouse()
    {
        wasMouseLockedBeforeDialogue = Cursor.lockState == CursorLockMode.Locked;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void RestoreMouseState()
    {
        if (wasMouseLockedBeforeDialogue)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void SetCanvasesActive(bool active)
    {
        if (canvasesToDisable == null) return;

        foreach (var canvas in canvasesToDisable)
        {
            if (canvas == null) continue;
            canvas.enabled = active;
        }
    }
}
