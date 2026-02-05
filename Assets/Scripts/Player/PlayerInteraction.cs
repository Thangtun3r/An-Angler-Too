using System;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float raycastDistance = 5f;

    private Camera mainCamera;
    private IPlayerInteraction currentInteractable;

    [HideInInspector] public bool isTalking = false;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!isTalking)
        {
            HandleInteraction();
        }
    }

    private void HandleInteraction()
    {
        // Get the ray from the center of the screen
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.red);

        IPlayerInteraction hitInteractable = null;

        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
        {
            hitInteractable = hit.collider.GetComponent<IPlayerInteraction>();
        }

        if (hitInteractable != currentInteractable)
        {
            if (currentInteractable != null)
            {
                currentInteractable.Unhighlight();
            }

            currentInteractable = hitInteractable;

            if (currentInteractable != null)
            {
                currentInteractable.Highlight();
            }
        }

        if (currentInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            currentInteractable.Interact();
        }
    }
}
