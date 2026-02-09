using UnityEngine;
using Yarn.Unity;
using FMODUnity;

public class YarnPuterSfxCommands : MonoBehaviour
{
    [YarnCommand("puterBomb")]
    public static void PuterBomb()
    {
        if (FMODEvents.Instance != null)
            PlayOneShot(FMODEvents.Instance.bomb);
    }

    private static void PlayOneShot(EventReference evt)
    {
        if (AudioManager.Instance == null || FMODEvents.Instance == null)
            return;

        AudioManager.Instance.PlayOneShot(evt, Vector3.zero);
    }
}
