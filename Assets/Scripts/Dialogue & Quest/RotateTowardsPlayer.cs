using UnityEngine;

public class RotateTowardsPlayer : MonoBehaviour
{
    private Transform target;

    void Start()
    {
        GameObject targetGameObject = GameObject.FindGameObjectWithTag("Player");
        target = targetGameObject.transform;
    }

    void Update()
    {
        Vector3 direction = target.position - transform.position;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(-direction.x, 0, -direction.z));
        }
    }
}
