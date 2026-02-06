using UnityEngine;

public class RotateXTowardsTarget : MonoBehaviour
{
    public Transform target;
    public float rotationSpeed = 5f;

    void Update()
    {
        if (target == null) return;

        // Direction from this object to the target
        Vector3 direction = target.position - transform.position;

        // Ignore Y and Z rotation, only care about X tilt
        float angleX = Mathf.Atan2(direction.y, direction.z) * Mathf.Rad2Deg;

        // Desired rotation (only X changes)
        Quaternion targetRotation = Quaternion.Euler(
            angleX,
            transform.eulerAngles.y,
            transform.eulerAngles.z
        );

        // Smoothly rotate
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}