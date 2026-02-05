using System.Collections.Generic;
using UnityEngine;

public class CollectionUIBuilder : MonoBehaviour
{
    public CollectionSlotUI slotPrefab;
    public Transform slotRoot;
    public List<ItemSO> allFish;

    void Start()
    {
        var manager = CollectionManager.Instance;

        foreach (var fish in allFish)
        {
            var slot = Instantiate(slotPrefab, slotRoot);
            slot.Setup(fish, manager);
        }
    }
}