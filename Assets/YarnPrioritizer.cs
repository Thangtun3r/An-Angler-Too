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

    private void Awake()
    {
        if (dialogueRunner == null)
            dialogueRunner = FindFirstObjectByType<DialogueRunner>();
    }

    private void OnEnable()
    {
        if (dialogueRunner == null) return;

        dialogueRunner.onNodeStart.AddListener(OnNodeStarted);
        dialogueRunner.onDialogueComplete.AddListener(OnDialogueCompleted);
    }

    private void OnDisable()
    {
        if (dialogueRunner == null) return;

        dialogueRunner.onNodeStart.RemoveListener(OnNodeStarted);
        dialogueRunner.onDialogueComplete.RemoveListener(OnDialogueCompleted);
    }

    private void OnNodeStarted(string nodeName)
    {
        if (unlockMouseOnDialogue)
            CursorLockManager.RequestUnlock("Dialogue");

        SetCanvasesActive(false);
    }

    private void OnDialogueCompleted()
    {
        if (unlockMouseOnDialogue)
            CursorLockManager.ReleaseUnlock("Dialogue");

        SetCanvasesActive(true);
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