using UnityEngine;
using Cinemachine;
using System.Collections;

public class StoreToggle : MonoBehaviour
{
    [SerializeField] private GameObject storeObj;
    [SerializeField] private Animator traderAnimator;

    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera storeCamera;
    [SerializeField] private int priorityOffset = 2;

    [Header("Timing")]
    [SerializeField] private float openDelay = 0.3f;

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
        StoreYarn.OnStoreStateChanged += HandleStoreState;
    }

    private void OnDisable()
    {
        StoreYarn.OnStoreStateChanged -= HandleStoreState;
    }

    private void HandleStoreState(bool open)
    {
        if (open)
            OpenStore();
        else
            CloseStore();
    }

    private void OpenStore()
    {
        if (storeObj.activeSelf) return;
        StartOpenDelayed();
    }

    private void CloseStore()
    {
        if (!storeObj.activeSelf) return;

        if (openRoutine != null)
        {
            StopCoroutine(openRoutine);
            openRoutine = null;
        }

        SetStore(false);
    }
    public void OnExitButtonPressed()
    {
        StoreYarn.CloseStore();
    }

    private void StartOpenDelayed()
    {
        if (openRoutine != null) return;
        openRoutine = StartCoroutine(OpenAfterDelay());
    }

    private IEnumerator OpenAfterDelay()
    {
        traderAnimator?.SetBool(IsToggleStore, true);
        UpdateCameraPriority(true);
        CursorLockManager.RequestUnlock("Shop");

        yield return new WaitForSeconds(openDelay);
        SetStore(true);
        openRoutine = null;
    }

    private void SetStore(bool open)
    {
        if (storeObj.activeSelf == open) return;

        storeObj.SetActive(open);

        if (!open)
        {
            traderAnimator?.SetBool(IsToggleStore, false);
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
