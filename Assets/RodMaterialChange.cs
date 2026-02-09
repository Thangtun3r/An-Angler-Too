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
    public float colliderSize;

    [Header("Fish Settings")]
    [Tooltip("Fish that spawns at the mirrored collider (legendary target, etc).")]
    public ItemSO fishToCatch;
    [Tooltip("Optional: override fish SO specifically for the mirrored guy; falls back to Fish To Catch if null.")]
    public ItemSO mirroredGuyFish;

    public Material alwaysOnTopMaterial;
    private GameObject mirrorCollider;

    private Vector3 mirroredPosition;

    void Awake()
    {
        GameObject sourceObject = GameObject.FindGameObjectWithTag("MainCamera");
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
        if (mirrorCollider != null)
            Destroy(mirrorCollider);

        mirrorCollider = new GameObject("MirroredGroundCollider");

        SphereCollider col = mirrorCollider.AddComponent<SphereCollider>();
        col.radius = colliderSize;
        col.isTrigger = false;

        int groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer == -1)
            Debug.LogError("Layer 'Ground' does not exist!");

        mirrorCollider.layer = groundLayer;

        FishContainer fishContainer = mirrorCollider.AddComponent<FishContainer>();
        ItemSO fish = mirroredGuyFish != null ? mirroredGuyFish : fishToCatch;

        if (fish == null)
        {
            Debug.LogWarning("RodMaterialChange: fishToCatch is not set. Bobber will land but no fish will be caught.");
        }
        else
        {
            fishContainer.fishSO = fish;
        }

        UpdateMirrorPosition();
    }

    void UpdateMirrorPosition()
    {
        if (mirrorCollider == null) return;

        // Mirror math
        mirroredPosition = center.position * 2f - player.position;

        mirroredPosition.y = player.position.y;
        mirrorCollider.transform.position = mirroredPosition;
    }

        void OnDrawGizmos()
    {
        if (center == null || player == null) return;

        // Recalculate in editor so it shows without Play mode
        Vector3 gizmoPos =
            center.position * 2f - player.position;
        gizmoPos.y = player.position.y;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(gizmoPos, colliderSize);

        // Optional: draw symmetry line
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(player.position, gizmoPos);
    }
}
