using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewFish", menuName = "Fishing/Fish", order = 1)]
public class ItemSO : ScriptableObject
{
    public bool isQuestItem;
    public string item_name;
    public Sprite item_sprite;
    public GameObject item_prefab;
    [TextArea(3, 10)]
    public string ItemDescription;
}

