using System;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public FishInventory Inventory { get; private set; }

    public Animator inventoryAnimator;

    private bool isOpen;

    public static event Action<bool> OnInventoryToggled;

    private void Awake()
    {
        Inventory = GetComponent<FishInventory>();

        if (inventoryAnimator != null)
        {
            inventoryAnimator.SetBool("isOpen", false);
        }

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

        OnInventoryToggled?.Invoke(isOpen);
    }
}