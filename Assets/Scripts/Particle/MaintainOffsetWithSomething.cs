using UnityEngine;

public class MaintainOffsetWithSomething : MonoBehaviour
{
    public Transform target;

    private Vector3 initialOffset;

    void Start()
    {
        if (target == null) return;

        initialOffset = transform.position - target.position;
    }

    void LateUpdate()
    {
        if (target == null) return;

        transform.position = target.position + initialOffset;
        transform.rotation = Quaternion.LookRotation(target.forward, Vector3.up);
    }
}