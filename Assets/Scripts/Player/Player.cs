using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerInteraction interaction;
    private FishingCast fishingCast;
    public CharacterController characterController;

    // Track who is currently locking the player
    private bool inventoryOpen;
    private bool shopOpen;

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
        StoreYarn.OnStoreOpened += HandleShopToggle;
    }

    private void OnDisable()
    {
        PlayerInventory.OnInventoryToggled -= HandleInventoryToggle;
        StoreYarn.OnStoreOpened -= HandleShopToggle;
    }

    private void Start()
    {
        // Default mouse state
        if (!CursorLockManager.IsUnlockedBySomeone)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // ---------------- Inventory ----------------

    private void HandleInventoryToggle(bool isOpen)
    {
        inventoryOpen = isOpen;
        RefreshPlayerState();
    }

    // ---------------- Shop ----------------

    private void HandleShopToggle()
    {
        // Shop toggle event has no bool → flip state
        shopOpen = !shopOpen;
        RefreshPlayerState();
    }

    // ---------------- Core Logic ----------------

    private void RefreshPlayerState()
    {
        if (inventoryOpen || shopOpen)
            DisablePlayer();
        else
            EnablePlayer();
    }

    public void DisablePlayer()
    {
        if (movement != null) movement.enabled = false;
        if (interaction != null) interaction.enabled = false;
        if (fishingCast != null) fishingCast.enabled = false;
        if (characterController != null) characterController.enabled = false;
    }

    public void EnablePlayer()
    {
        if (movement != null) movement.enabled = true;
        if (interaction != null) interaction.enabled = true;
        if (fishingCast != null) fishingCast.enabled = true;
        if (characterController != null) characterController.enabled = true;
    }

    // ---------------- Partial Control ----------------

    public void FreezeMovementOnly()
    {
        if (movement != null)
            movement.IsFrozen = true;
    }

    public void UnFreezeMovementOnly()
    {
        if (movement != null)
            movement.IsFrozen = false;
    }

    // ---------------- Spawn ----------------

    public void SetPlayerSpawnPoint(Transform spawnPoint)
    {
        if (characterController == null) return;

        characterController.enabled = false;

        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        if (movement != null)
            movement.ResetHead();

        characterController.enabled = true;
    }
}
