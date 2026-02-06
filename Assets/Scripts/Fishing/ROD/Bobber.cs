using System;
using UnityEngine;

public class Bobber : MonoBehaviour
{
    public static event Action OnBobberLanded;
    public IFish currentFish;

    [Header("Detection Settings")]
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    
    private Rigidbody rb;
    private bool hasLanded;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    private void FixedUpdate()
    {
        if (hasLanded) return;
        
        float stepDistance = rb.velocity.magnitude * Time.fixedDeltaTime;
        Vector3 moveDirection = rb.velocity.normalized;
        float castDistance = Mathf.Max(stepDistance, 0.1f);
        if (Physics.SphereCast(transform.position, groundCheckRadius, moveDirection,
                out RaycastHit hit, castDistance, groundLayer))
        {
            HandleLanding(hit);
        }
    }

    private void HandleLanding(RaycastHit hit)
    {
        hasLanded = true;

        transform.position = hit.point + (hit.normal * groundCheckRadius);

        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        currentFish = hit.collider.GetComponent<IFish>();
        
        if (currentFish != null)
        {
            currentFish.BobberLanded(transform);
        }
        OnBobberLanded?.Invoke();
    }

    public void ResetBobber()
    {
        rb.isKinematic = false;
        hasLanded = false;
        currentFish = null;
    }

    private void OnDrawGizmos()
    {
        if (rb == null) return;

        Gizmos.color = Color.red;
        Vector3 direction = rb.velocity.normalized;
        float dist = Mathf.Max(rb.velocity.magnitude * Time.fixedDeltaTime, 0.5f);
        Gizmos.DrawWireSphere(transform.position + (direction * dist), groundCheckRadius);
        Gizmos.DrawLine(transform.position, transform.position + (direction * dist));
    }
}