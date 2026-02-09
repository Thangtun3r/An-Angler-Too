using System.Collections;
using UnityEngine;
using Yarn.Unity;

public class OneTimeTeleport : MonoBehaviour
{
    private GameObject player;
    private GameObject bobber; 
    public GameObject teleportMarker;

    void Awake()
    {
        teleportMarker = GameObject.FindGameObjectWithTag("Marker");
        player = GameObject.FindGameObjectWithTag("Player");
        bobber = GameObject.FindGameObjectWithTag("Bobber");
    }

    [YarnCommand ("teleport")]
    public void Teleport()
    {
        Transform p = player.transform;
        Transform m = teleportMarker.transform;
        Transform b = bobber.transform;

        p.SetPositionAndRotation(m.position, m.rotation);
        b.SetPositionAndRotation(m.position, b.rotation);

        Player playerScript  = player.GetComponent<Player>();
        playerScript.EnablePlayer();

    }
}