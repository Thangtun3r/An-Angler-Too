using UnityEngine;
using UnityEngine.EventSystems;
using FMODUnity;

public class UIAudioClickListener : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;

    private void Awake()
    {
        if (playerMovement == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerMovement = player.GetComponent<PlayerMovement>();
        }
    }

    private void Update()
    {
        if (playerMovement != null && playerMovement.enabled)
            return;

        if (!Input.GetMouseButtonDown(0))
            return;

        if (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject())
            return;

        var events = FMODEvents.Instance;
        if (events == null || AudioManager.Instance == null)
            return;

        AudioManager.Instance.PlayOneShot(events.uiClick, transform.position);
    }
}
