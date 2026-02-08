using UnityEngine;
using Cinemachine;

public class StoreToggle : MonoBehaviour
{
    [SerializeField] private GameObject StoreObj;
    [SerializeField] private Animator traderAnimator;

    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera storeCamera;
    [SerializeField] private int priorityOffset = 2;

    private int baseCameraPriority;

    private static readonly int IsToggleStore = Animator.StringToHash("isToggleStore");

    private void Awake()
    {
        if (storeCamera != null)
            baseCameraPriority = storeCamera.Priority;
    }

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
        if (StoreObj == null) return;

        bool opening = !StoreObj.activeSelf;
        SetStore(opening);
    }

    public void CloseStore() => SetStore(false);
    public void OpenStore() => SetStore(true);

    private void SetStore(bool open)
    {
        if (StoreObj.activeSelf == open) return;

        StoreObj.SetActive(open);

        if (traderAnimator != null)
            traderAnimator.SetBool(IsToggleStore, open);

        UpdateCameraPriority(open);

        if (open)
            CursorLockManager.RequestUnlock("Shop");
        else
            CursorLockManager.ReleaseUnlock("Shop");
    }

    private void UpdateCameraPriority(bool open)
    {
        if (storeCamera == null) return;

        storeCamera.Priority = open
            ? baseCameraPriority + priorityOffset
            : baseCameraPriority;
    }
}