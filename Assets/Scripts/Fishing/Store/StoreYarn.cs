using UnityEngine;
using Yarn.Unity;

public class StoreYarn : MonoBehaviour
{
    public static event System.Action<bool> OnStoreStateChanged;

    [YarnCommand("open_store")]
    public static void OpenStore()
    {
        OnStoreStateChanged?.Invoke(true);
    }

    // ✅ CALLED BY EXIT TRIGGER / UI
    public static void CloseStore()
    {
        OnStoreStateChanged?.Invoke(false);
    }
}