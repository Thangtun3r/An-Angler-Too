using System;
using UnityEngine;
using FMODUnity;

public class PlayerInventory : MonoBehaviour
{
    public FishInventory Inventory { get; private set; }
    public Animator inventoryAnimator;

    private bool isOpen;

    public static event Action<bool> OnInventoryToggled;
    public static event Action OnInventoryClosed;

    private void Awake()
    {
        Inventory = GetComponent<FishInventory>();

        if (inventoryAnimator != null)
            inventoryAnimator.SetBool("isOpen", false);

        isOpen = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
    }

    private void ToggleInventory()
    {
        if (inventoryAnimator == null) return;

        isOpen = !isOpen;
        inventoryAnimator.SetBool("isOpen", isOpen);
        if (FMODEvents.Instance != null)
        {
            PlayUiOneShot(isOpen ? FMODEvents.Instance.uiBoxOpen : FMODEvents.Instance.uiBoxClose);
        }

        // Cursor handling (prevents overlap fights)
        if (isOpen)
            CursorLockManager.RequestUnlock("Inventory");
        else
            CursorLockManager.ReleaseUnlock("Inventory");

        OnInventoryToggled?.Invoke(isOpen);

        if (!isOpen)
            OnInventoryClosed?.Invoke();
    }

    private void PlayUiOneShot(EventReference evt)
    {
        if (AudioManager.Instance == null || FMODEvents.Instance == null)
            return;

        AudioManager.Instance.PlayOneShot(evt, transform.position);
    }
}
