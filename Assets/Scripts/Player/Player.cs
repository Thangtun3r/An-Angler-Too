using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerInteraction interaction;
    public CharacterController characterController;
    private FishingCast fishingCast;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        interaction = GetComponent<PlayerInteraction>();
        characterController = GetComponent<CharacterController>();
        fishingCast = GetComponent<FishingCast>(); 
        
    }

    private void OnEnable()
    {
        PlayerInventory.OnInventoryToggled += HandleInventoryToggle;
    }

    private void OnDisable()
    {
        PlayerInventory.OnInventoryToggled -= HandleInventoryToggle;
    }
    
    private void HandleInventoryToggle(bool isOpen)
    {
        if (isOpen)
        {
            DisablePlayer();
            UnlockMouse();
        }
        else
        {
            EnablePlayer();
            LockMouse();
        }
    }

    private void Start()
    {
        LockMouse();
    }

    public void DisablePlayer()
    {
        movement.enabled = false;
        interaction.enabled = false;
        characterController.enabled = false;
        fishingCast.enabled = false;
    }

    public void EnablePlayer()
    {
        movement.enabled = true;
        interaction.enabled = true;
        characterController.enabled = true;
        fishingCast.enabled = true;
    }

    public void FreezeMovementOnly()
    {
        movement.IsFrozen = true;
    }

    public void UnFreezeMovementOnly()
    {
        movement.IsFrozen = false;
    }

    public void LockMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SetPlayerSpawnPoint(Transform spawnPoint)
    {
        characterController.enabled = false;
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;
        movement.ResetHead();
        characterController.enabled = true;
    }
}