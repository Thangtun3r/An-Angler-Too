using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CollectionSlotUI : MonoBehaviour, IPointerClickHandler
{
    public Image icon;

    private ItemSO data;
    private CollectionManager manager;

    public void Setup(ItemSO item, CollectionManager collectionManager)
    {
        data = item;
        manager = collectionManager;

        Refresh();
        manager.OnFishDiscovered += OnFishDiscovered;
    }

    private void OnDestroy()
    {
        if (manager != null)
            manager.OnFishDiscovered -= OnFishDiscovered;
    }

    private void OnFishDiscovered(ItemSO item)
    {
        if (item.itemID == data.itemID)
        {
            Refresh();
        }
    }

    private void Refresh()
    {
        manager.GetDisplaySprite(data, out Sprite sprite);
        icon.sprite = sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        manager.ShowInfo(data);
    }
}