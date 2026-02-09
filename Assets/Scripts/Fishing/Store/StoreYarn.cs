using UnityEngine;
using Yarn.Unity;

public class StoreYarn : MonoBehaviour
{
    public static event System.Action<bool> OnStoreStateChanged;

    [YarnCommand("open_store")]
    public static void OpenStore()
    {
        OnStoreStateChanged?.Invoke(true);
        PlayShopkeeperTalk();
    }

    // ✅ CALLED BY EXIT TRIGGER / UI
    public static void CloseStore()
    {
        OnStoreStateChanged?.Invoke(false);
    }

    private static void PlayShopkeeperTalk()
    {
        if (AudioManager.Instance == null || FMODEvents.Instance == null)
            return;

        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.shopkeeperTalk, Vector3.zero);
    }
}
