using UnityEngine;

public class HoverBoard_Equipment : MonoBehaviour, IEquipment
{
    [Header("References")]
    [SerializeField] private GameObject hoverboard;
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Stats")]
    [SerializeField] private float speedMultiplier = 1.5f;

    private bool isEquipped;
    private float cachedBaseSpeed;

    private void Awake()
    {
        if (hoverboard != null)
            hoverboard.SetActive(false);

        CacheBaseSpeed();
    }

    private void CacheBaseSpeed()
    {
        if (playerMovement != null)
            cachedBaseSpeed = playerMovement.speed;
    }

    /// <summary>
    /// Called by the equipment system
    /// </summary>
    public void SetEquipped(bool equipped)
    {
        if (isEquipped == equipped)
            return;

        isEquipped = equipped;

        if (playerMovement != null)
        {
            // Re-cache base speed in case something else modified it
            CacheBaseSpeed();

            playerMovement.speed = isEquipped
                ? cachedBaseSpeed * speedMultiplier
                : cachedBaseSpeed;
        }

        if (hoverboard != null)
            hoverboard.SetActive(isEquipped);
    }

    public bool IsEquipped() => isEquipped;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (speedMultiplier < 1f)
            speedMultiplier = 1f;
    }
#endif
}