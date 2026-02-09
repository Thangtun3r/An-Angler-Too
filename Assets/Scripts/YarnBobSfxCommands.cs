using UnityEngine;
using Yarn.Unity;
using FMODUnity;

public static class YarnBobSfxCommands
{
    [YarnCommand("bobTalk1")]
    public static void BobTalk1()
    {
        if (FMODEvents.Instance != null)
            PlayOneShot(FMODEvents.Instance.bobTalk1);
    }

    private static void PlayOneShot(EventReference evt)
    {
        if (AudioManager.Instance == null || FMODEvents.Instance == null)
            return;

        AudioManager.Instance.PlayOneShot(evt, Vector3.zero);
    }
}
