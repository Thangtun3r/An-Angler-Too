using UnityEngine;

public class StoreToggle : MonoBehaviour
{
    [SerializeField] private GameObject StoreObj;

    private void OnEnable()
    {
        StoreYarn.OnStoreOpened += ToggleStore;
    }

    private void OnDisable()
    {
        StoreYarn.OnStoreOpened -= ToggleStore;
    }

    private void ToggleStore()
    {
        if (StoreObj == null)
        {
            Debug.LogWarning("StoreObj is not assigned!");
            return;
        }

        bool opening = !StoreObj.activeSelf;
        SetStore(opening);
    }

    /// <summary>
    /// Public method to force-close the store (safe to call anytime)
    /// </summary>
    public void CloseStore()
    {
        SetStore(false);
    }

    /// <summary>
    /// Optional: force open if you ever need it
    /// </summary>
    public void OpenStore()
    {
        SetStore(true);
    }

    private void SetStore(bool open)
    {
        if (StoreObj == null) return;

        // Already in desired state → do nothing
        if (StoreObj.activeSelf == open) return;

        StoreObj.SetActive(open);

        if (open)
            CursorLockManager.RequestUnlock("Shop");
        else
            CursorLockManager.ReleaseUnlock("Shop");
    }
}