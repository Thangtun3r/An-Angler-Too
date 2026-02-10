using UnityEngine;

public class HoverBoard_Equipment : MonoBehaviour, IEquipment
{
    [Header("References")]
    [SerializeField] private GameObject hoverboard;
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Stats")]
    [SerializeField] private float speedMultiplier = 1.5f;

    private bool isEquipped;
    private float baseSpeed; // true original speed

    private void Awake()
    {
        if (hoverboard != null)
            hoverboard.SetActive(false);

        if (playerMovement != null)
            baseSpeed = playerMovement.speed;
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
            playerMovement.speed = isEquipped
                ? baseSpeed * speedMultiplier
                : baseSpeed;
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