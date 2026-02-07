using UnityEngine;

public class HandAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string isStoringParam = "isStoring";

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        {
            FishContainer.OnStoreStarted += OnStoreStarted;
        }
    }

    private void OnDisable()
    {
        FishContainer.OnStoreStarted -= OnStoreStarted;
    }

    private void OnStoreStarted()
    {
            animator.SetBool(isStoringParam, true);
    }
    
}