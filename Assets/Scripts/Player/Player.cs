using UnityEngine;
using System.Diagnostics;

public enum PlayerLockSource
{
    Unknown,
    Inventory,
    Shop,
    System
}

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
        StoreYarn.OnStoreStateChanged += HandleShopState;
    }

    private void OnDisable()
    {
        PlayerInventory.OnInventoryToggled -= HandleInventoryToggle;
        StoreYarn.OnStoreStateChanged -= HandleShopState;
    }

    private void HandleShopState(bool isOpen)
    {
        shopOpen = isOpen;
        RefreshPlayerState();
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

        // FIX: inventory opening clears shop lock
        if (inventoryOpen && shopOpen)
            shopOpen = false;

        RefreshPlayerState();
    }

    // ---------------- Shop ----------------

    private void HandleShopToggle()
    {
        shopOpen = !shopOpen;

        // FIX: shop opening clears inventory lock
        if (shopOpen && inventoryOpen)
            inventoryOpen = false;

        RefreshPlayerState();
    }

    // ---------------- Core Logic ----------------

    private void RefreshPlayerState()
    {
        if (inventoryOpen || shopOpen)
        {
            DisablePlayer(
                inventoryOpen ? PlayerLockSource.Inventory : PlayerLockSource.Shop
            );
        }
        else
        {
            EnablePlayer(
                inventoryOpen ? PlayerLockSource.Inventory :
                shopOpen ? PlayerLockSource.Shop :
                PlayerLockSource.System
            );
        }

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
        // 🔒 Guard: don't allow enable if something still owns the lock
        if (inventoryOpen || shopOpen)
        {
            UnityEngine.Debug.Log(
                $"<color=yellow>[Player ENABLE BLOCKED]</color> by <b>{source}</b> " +
                $"(inventoryOpen={inventoryOpen}, shopOpen={shopOpen})"
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


    // ---------------- Partial Control (DEPENDENCY SAFE) ----------------

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
