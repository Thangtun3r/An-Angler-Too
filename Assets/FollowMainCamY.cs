using UnityEngine;

public class FollowMainCamY : MonoBehaviour
{
    private Transform cam;

    void Start()
    {
        if (Camera.main != null)
            cam = Camera.main.transform;
        else
            Debug.LogError("No Main Camera found!");
    }

    void LateUpdate()
    {
        if (cam == null) return;

        Vector3 pos = transform.position;
        pos.y = cam.position.y;
        transform.position = pos;
    }
}