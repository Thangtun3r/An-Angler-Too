using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddFishTest : MonoBehaviour
{
    public FishInventory fishInventory;
    public ItemSO testFish;
    private FishInventory playerInventory;

    void Start()
    {
        playerInventory = FindObjectOfType<PlayerInventory>().Inventory;
    }
    public void AddTestFish()
    {
        playerInventory.AddItem(testFish);
    }
}
