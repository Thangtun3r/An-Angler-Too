using UnityEngine;
using Yarn.Unity;

public class StopFirstPersonOnDialogue : MonoBehaviour
{
    private DialogueRunner dialogueRunner;
    private Player player;
    private PlayerMovement playerMovement;
    private FishingCast fishingCast;
    private PlayerInteraction playerInteraction;

    void Awake()
    {
        dialogueRunner = FindFirstObjectByType<DialogueRunner>(FindObjectsInactive.Include);

        var playerGO = GameObject.FindGameObjectWithTag("Player");
        player = playerGO.GetComponent<Player>();
        playerMovement = playerGO.GetComponent<PlayerMovement>();
        fishingCast = playerGO.GetComponent<FishingCast>();
        playerInteraction = playerGO.GetComponent<PlayerInteraction>();
    }

    void OnEnable()
    {
        if (dialogueRunner == null)
        {
            //Debug.LogError("StopFirstPersonOnDialogue: no DialogueRunner in scene");
            enabled = false;
            return;
        }

        dialogueRunner.onDialogueStart.AddListener(HandleDialogueStart);
        dialogueRunner.onDialogueComplete.AddListener(HandleDialogueEnd);
    }

    void OnDisable()
    {
        if (dialogueRunner == null) return;
        dialogueRunner.onDialogueStart.RemoveListener(HandleDialogueStart);
        dialogueRunner.onDialogueComplete.RemoveListener(HandleDialogueEnd);
    }

    private void HandleDialogueStart()
    {
        SetTalking(true);
    }

    private void HandleDialogueEnd()
    {
        SetTalking(false);
    }

    private void SetTalking(bool talking)
    {
        if (talking)
        {
            playerMovement.isTalking = true;
            playerInteraction.isTalking = true;
            fishingCast.isTalking = true;
            player.FreezeMovementOnly();
            player.DisablePlayer();
        }
        else
        {
            playerMovement.isTalking = false;
            playerInteraction.isTalking = false;
            fishingCast.isTalking = false;
            player.UnFreezeMovementOnly();
            player.EnablePlayer();
        }
    }
}
