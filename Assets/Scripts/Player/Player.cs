using UnityEngine;
using System.Diagnostics;

public enum PlayerLockSource
{
    Unknown,
    Inventory,
    Shop,
    System,
    PauseMenu
}

public class Player : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerInteraction interaction;
    private FishingCast fishingCast;
    public CharacterController characterController;

    // Lock flags (OWNED ONLY by their systems)
    private bool inventoryOpen;
    private bool shopOpen;
    private bool pauseMenuOpen;

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
        StoreYarn.OnStoreStateChanged += HandleShopState;
    }

    private void OnDisable()
    {
        PlayerInventory.OnInventoryToggled -= HandleInventoryToggle;
        StoreYarn.OnStoreStateChanged -= HandleShopState;
    }

    private void Start()
    {
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

    private void HandleShopState(bool isOpen)
    {
        shopOpen = isOpen;
        RefreshPlayerState();
    }

    // ---------------- Pause Menu ----------------

    private void HandlePauseMenuState(bool isOpen)
    {
        pauseMenuOpen = isOpen;
        RefreshPlayerState();
    }

    // ---------------- Core Logic ----------------

    private void RefreshPlayerState()
    {
        // Shop has highest priority
        if (shopOpen)
        {
            DisablePlayer(PlayerLockSource.Shop);
            return;
        }

        // Pause menu is second priority
        if (pauseMenuOpen)
        {
            DisablePlayer(PlayerLockSource.PauseMenu);
            return;
        }

        // Inventory is third priority
        if (inventoryOpen)
        {
            DisablePlayer(PlayerLockSource.Inventory);
            return;
        }

        // No locks remaining
        EnablePlayer(PlayerLockSource.System);
    }

    // ---------------- Player Control ----------------

    public void DisablePlayer(PlayerLockSource source = PlayerLockSource.Unknown)
    {
        UnityEngine.Debug.Log(
            $"<color=red>[Player DISABLED]</color> by <b>{source}</b>\n{GetStackTrace()}"
        );

        if (movement != null) movement.enabled = false;
        if (interaction != null) interaction.enabled = false;
        if (fishingCast != null) fishingCast.enabled = false;
        if (characterController != null) characterController.enabled = false;
    }

    public void EnablePlayer(PlayerLockSource source = PlayerLockSource.Unknown)
    {
        // Absolute safety guard (should never hit now)
        if (shopOpen || inventoryOpen || pauseMenuOpen)
        {
            UnityEngine.Debug.Log(
                $"<color=yellow>[Player ENABLE BLOCKED]</color> by <b>{source}</b> " +
                $"(inventoryOpen={inventoryOpen}, shopOpen={shopOpen}, pauseMenuOpen={pauseMenuOpen})"
            );
            return;
        }

        UnityEngine.Debug.Log(
            $"<color=green>[Player ENABLED]</color> by <b>{source}</b>\n{GetStackTrace()}"
        );

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

    // ---------------- Debug Helper ----------------

    private string GetStackTrace()
    {
        return new StackTrace(2, true).ToString();
    }
}



