using System.Collections.Generic;
using UnityEngine;

public class CollectionUIBuilder : MonoBehaviour
{
    public CollectionSlotUI slotPrefab;
    public Transform slotRoot;
    public List<ItemSO> allFish;

    private void Start()
    {
        if (slotPrefab == null || slotRoot == null)
        {
            Debug.LogError("CollectionUIBuilder missing slotPrefab or slotRoot.");
            return;
        }

        var manager = CollectionManager.Instance;
        if (manager == null)
        {
            Debug.LogError("CollectionManager.Instance not found in scene.");
            return;
        }

        if (allFish == null) return;

        foreach (var fish in allFish)
        {
            if (fish == null) continue;

            var slot = Instantiate(slotPrefab, slotRoot);
            slot.Setup(fish, manager);
        }
    }
}