using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Yarn.Unity;

public class StopFirstPersonOnDialogue: MonoBehaviour
{
    private Player player;
    private PlayerMovement playerMovement;
    private FishingCast fishingCast;
    private PlayerInteraction playerInteraction;
    private DialogueRunner dialogueRunner;

    void Awake()
    {
        dialogueRunner = GameObject.FindFirstObjectByType<DialogueRunner>();
        GameObject playerGameObject = GameObject.FindGameObjectWithTag("Player");
        player = playerGameObject.GetComponent<Player>();
        playerMovement = playerGameObject.GetComponent<PlayerMovement>();
        fishingCast = playerGameObject.GetComponent<FishingCast>();
        playerInteraction = playerGameObject.GetComponent<PlayerInteraction>();

        Debug.Log(dialogueRunner.gameObject.name);
    } 

    public void DisablePlayer()
    {
        //In order: free mouse, stop cam from following mouse, stop click from triggering dialogue, stop fishing rod from casting
        player.UnlockMouse();
        playerMovement.isTalking = true;
        playerInteraction.isTalking = true;
        fishingCast.isTalking = true;
        player.FreezeMovementOnly();
        player.DisablePlayer();
    }

    public void ReturnPlayer()
    {
        player.LockMouse();
        playerMovement.isTalking = false;
        playerInteraction.isTalking = false;
        fishingCast.isTalking = false;
        player.UnFreezeMovementOnly();
        player.EnablePlayer();
    }

    void OnEnable()
    {
        dialogueRunner.onDialogueComplete.AddListener(ReturnPlayer);
        dialogueRunner.onDialogueStart.AddListener(DisablePlayer);
    }

    void OnDisable()
    {
        dialogueRunner.onDialogueComplete.RemoveListener(ReturnPlayer);
        dialogueRunner.onDialogueStart.RemoveListener(DisablePlayer);
    }
}
