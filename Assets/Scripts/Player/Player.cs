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
        if (isOpen) DisablePlayer();
        else EnablePlayer();
    }

    private void Start()
    {
        // Default: lock mouse only if no UI is requesting it unlocked
        if (!CursorLockManager.IsUnlockedBySomeone)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void DisablePlayer()
    {
        if (movement != null) movement.enabled = false;
        if (interaction != null) interaction.enabled = false;
        if (characterController != null) characterController.enabled = false;
        if (fishingCast != null) fishingCast.enabled = false;
    }

    public void EnablePlayer()
    {
        if (movement != null) movement.enabled = true;
        if (interaction != null) interaction.enabled = true;
        if (characterController != null) characterController.enabled = true;
        if (fishingCast != null) fishingCast.enabled = true;
    }

    public void FreezeMovementOnly()
    {
        if (movement != null) movement.IsFrozen = true;
    }

    public void UnFreezeMovementOnly()
    {
        if (movement != null) movement.IsFrozen = false;
    }

    public void SetPlayerSpawnPoint(Transform spawnPoint)
    {
        if (characterController == null) return;

        characterController.enabled = false;
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        if (movement != null) movement.ResetHead();

        characterController.enabled = true;
    }
}
