using UnityEngine;
using Yarn.Unity;

public class OneTimeTeleport : MonoBehaviour
{
    private GameObject player;
    public GameObject teleportMarker;

    void Awake()
    {
        teleportMarker = GameObject.FindGameObjectWithTag("Marker");
        player= GameObject.FindGameObjectWithTag("Player");
    }

    [YarnCommand ("teleport")]
    public void Teleport()
    {
        Transform p = player.transform;
        Transform m = teleportMarker.transform;

        p.SetPositionAndRotation(m.position, m.rotation);
    }
}