using UnityEngine;
using Yarn.Unity;
using FMODUnity;

public class YarnCatSfxCommands : MonoBehaviour
{
    [YarnCommand("catMeow")]
    public static void CatMeow()
    {
        if (FMODEvents.Instance != null)
            PlayCatOneShot(FMODEvents.Instance.catMeow);
    }

    [YarnCommand("catPurr")]
    public static void CatPurr()
    {
        if (FMODEvents.Instance != null)
            PlayCatOneShot(FMODEvents.Instance.catPurr);
    }

    [YarnCommand("catHiss")]
    public static void CatHiss()
    {
        if (FMODEvents.Instance != null)
            PlayCatOneShot(FMODEvents.Instance.catHiss);
    }

    [YarnCommand("catCuteMeow")]
    public static void CatCuteMeow()
    {
        if (FMODEvents.Instance != null)
            PlayCatOneShot(FMODEvents.Instance.catCuteMeow);
    }

    [YarnCommand("catTeleport")]
    public static void CatTeleport()
    {
        if (FMODEvents.Instance != null)
            PlayCatOneShot(FMODEvents.Instance.catTeleport);
    }

    private static void PlayCatOneShot(EventReference evt)
    {
        if (AudioManager.Instance == null || FMODEvents.Instance == null)
            return;

        AudioManager.Instance.PlayOneShot(evt, Vector3.zero);
    }
}
