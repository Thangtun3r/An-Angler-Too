using UnityEngine;
using Yarn.Unity;
using FMODUnity;

public static class YarnShopkeeperSfxCommands
{
    [YarnCommand("shopkeeperTalk")]
    public static void ShopkeeperTalk()
    {
        if (FMODEvents.Instance != null)
            PlayOneShot(FMODEvents.Instance.shopkeeperTalk);
    }

    private static void PlayOneShot(EventReference evt)
    {
        if (AudioManager.Instance == null || FMODEvents.Instance == null)
            return;

        AudioManager.Instance.PlayOneShot(evt, Vector3.zero);
    }
}
