using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class CheckIfRod : MonoBehaviour
{
    [SerializeField] private ItemSO item;
    [SerializeField] private string node;
    private DialogueRunner dialogueRunner;
    private bool pendingStart;
    private bool alreadyTriggered;

    void Awake()
    {
        dialogueRunner = GameObject.FindFirstObjectByType<DialogueRunner>();
    }

    void OnEnable()
    {
        InventoryService.OnItemAdded += HandleItemAdded;
        FishInventory.OnInventoryChanged += HandleInventoryChanged;
        dialogueRunner.onDialogueComplete?.AddListener(OnDialogueComplete);
    }

    void OnDisable()
    {
        InventoryService.OnItemAdded -= HandleItemAdded;
        FishInventory.OnInventoryChanged -= HandleInventoryChanged;
        dialogueRunner.onDialogueComplete?.RemoveListener(OnDialogueComplete);
    }

    private void HandleItemAdded(ItemSO addedItem)
    {
        if (addedItem != item || alreadyTriggered) return;

        TriggerDialogue();
    }

    private void OnDialogueComplete()
    {
        if (pendingStart && dialogueRunner != null)
        {
            pendingStart = false;
            dialogueRunner.StartDialogue(node);
        }
    }

    private void HandleInventoryChanged()
    {
        if (alreadyTriggered) return;

        FishInventory inv = InventoryService.PlayerInventory;
        if (inv == null) return;

        if (inv.Contains(item))
        {
            TriggerDialogue();
        }
    }

    private void TriggerDialogue()
    {
        alreadyTriggered = true;

        // Force-close the shop so the player is out of the UI (same as pressing Tab/Exit)
        StoreYarn.CloseStore();

        if (dialogueRunner.IsDialogueRunning)
        {
            pendingStart = true;
        }
        else
        {
            dialogueRunner.StartDialogue(node);
        }
    }
}
