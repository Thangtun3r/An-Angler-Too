using UnityEngine;
using Cinemachine;
using System.Collections;

public class StoreToggle : MonoBehaviour
{
    [SerializeField] private GameObject StoreObj;
    [SerializeField] private Animator traderAnimator;

    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera storeCamera;
    [SerializeField] private int priorityOffset = 2;

    [Header("Timing")]
    [SerializeField] private float openDelay = 0.3f; // 👈 delay before store shows

    private int baseCameraPriority;
    private Coroutine openRoutine;

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

        if (opening)
            StartOpenDelayed();
        else
            CloseStore();
    }

    public void OpenStore()
    {
        StartOpenDelayed();
    }

    public void CloseStore()
    {
        // cancel pending open
        if (openRoutine != null)
        {
            StopCoroutine(openRoutine);
            openRoutine = null;
        }

        SetStore(false);
    }

    // ---------------- Delay Logic ----------------

    private void StartOpenDelayed()
    {
        if (openRoutine != null) return; // already opening

        openRoutine = StartCoroutine(OpenAfterDelay());
    }

    private IEnumerator OpenAfterDelay()
    {
        // Animator + camera react immediately
        if (traderAnimator != null)
            traderAnimator.SetBool(IsToggleStore, true);

        UpdateCameraPriority(true);
        CursorLockManager.RequestUnlock("Shop");

        yield return new WaitForSeconds(openDelay);

        SetStore(true);

        openRoutine = null;
    }

    // ---------------- Core ----------------

    private void SetStore(bool open)
    {
        if (StoreObj.activeSelf == open) return;

        StoreObj.SetActive(open);

        if (!open)
        {
            if (traderAnimator != null)
                traderAnimator.SetBool(IsToggleStore, false);

            UpdateCameraPriority(false);
            CursorLockManager.ReleaseUnlock("Shop");
        }
    }

    private void UpdateCameraPriority(bool open)
    {
        if (storeCamera == null) return;

        storeCamera.Priority = open
            ? baseCameraPriority + priorityOffset
            : baseCameraPriority;
    }
}
