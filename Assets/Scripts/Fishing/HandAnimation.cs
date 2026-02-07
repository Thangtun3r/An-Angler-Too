using UnityEngine;

public class HandAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string isStoringParam = "isStoring";
    [SerializeField] private Transform handHold;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        FishContainer.OnStoreStarted += OnStoreStarted;
    }

    private void OnDisable()
    {
        FishContainer.OnStoreStarted -= OnStoreStarted;
    }

    private void OnStoreStarted()
    {
        animator.SetBool(isStoringParam, true);
    }
    public void OnStoreFinished()
    {
        if (animator != null)
            animator.SetBool(isStoringParam, false);

        if (handHold == null) return;
        for (int i = handHold.childCount - 1; i >= 0; i--)
        {
            Destroy(handHold.GetChild(i).gameObject);
        }
    }
}