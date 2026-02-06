using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterial : MonoBehaviour
{
    public int layerMask;
    public Material material;
    [Yarn.Unity.YarnCommand("changeMaterial")]
    public void ChangeMat()
    {
        // Set layer
        gameObject.layer = layerMask;

        // Set material (instance material, not shared)
        Renderer renderer = GetComponent<Renderer>();
        renderer.material = material;
    }

}
