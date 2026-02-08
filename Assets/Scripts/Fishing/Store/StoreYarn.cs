using UnityEngine;
using Yarn.Unity;

public class StoreYarn: MonoBehaviour
{
    public static event System.Action OnStoreOpened;

    [YarnCommand("open_store")]
    public static void OpenStoreCommand()
    {
        OnStoreOpened?.Invoke();
    }
}

