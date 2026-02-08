using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class RodMaterialChange : MonoBehaviour
{
    [Header("Objects to affect")]
    public List<GameObject> rod;

   [Header("References")]
    public Transform center;          // Mirror center
    public Transform player;          // You

    [Header("Collider Settings")]
    public Vector3 colliderSize = new Vector3(2f, 2f, 2f);

    public Material alwaysOnTopMaterial;
    private GameObject mirrorCollider;

    private Vector3 mirroredPosition;

    void Awake()
    {
        GameObject sourceObject = GameObject.FindGameObjectWithTag("Player");
        player = sourceObject.transform;

        GameObject mirrorPlaneObject = GameObject.FindGameObjectWithTag("Mirror");
        center = mirrorPlaneObject.transform;

    }

    [YarnCommand("changeRodMat")]
    public void ApplyMaterial()
    {
        foreach (GameObject obj in rod)
        {
            if (obj == null) continue;

            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer == null) continue;

            renderer.material = alwaysOnTopMaterial;
        }

        CreateCollider();
    }


    void LateUpdate()
    {
        UpdateMirrorPosition();
    }

    void CreateCollider()
    {
        mirrorCollider = new GameObject("MirroredGroundCollider");

        BoxCollider col = mirrorCollider.AddComponent<BoxCollider>();
        col.size = colliderSize;
        col.isTrigger = false;

        int groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer == -1)
            Debug.LogError("Layer 'Ground' does not exist!");

        mirrorCollider.layer = groundLayer;
    }

    void UpdateMirrorPosition()
    {
        if (center == null || player == null) return;

        // Mirror math
        Vector3 mirroredPosition =
            center.position * 2f - player.position;

        mirroredPosition.y = player.position.y;
        mirrorCollider.transform.position = mirroredPosition;

    }
}
