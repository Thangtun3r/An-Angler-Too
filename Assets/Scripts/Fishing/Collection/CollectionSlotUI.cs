using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CollectionSlotUI : MonoBehaviour, IPointerClickHandler
{
    [Header("UI")]
    public Image icon;
    public TMP_Text nameText;

    [Header("Locked Display")]
    public Sprite lockedSprite;
    public string lockedName = "???";

    private ItemSO data;
    private CollectionManager manager;

    public void Setup(ItemSO item, CollectionManager collectionManager)
    {
        data = item;
        manager = collectionManager;

        Refresh();

        if (manager != null)
            manager.OnFishDiscovered += OnFishDiscovered;
    }

    private void OnDestroy()
    {
        if (manager != null)
            manager.OnFishDiscovered -= OnFishDiscovered;
    }

    private void OnFishDiscovered(ItemSO item)
    {
        if (item == null || data == null) return;
        if (item.itemID == data.itemID)
            Refresh();
    }

    private void Refresh()
    {
        if (data == null) return;

        bool unlocked = manager != null && manager.IsUnlocked(data);

        // Icon
        if (icon != null)
        {
            if (unlocked)
            {
                icon.sprite = data.item_sprite;
            }
            else
            {
                // Prefer local lockedSprite if set, otherwise fallback to manager's display sprite
                if (lockedSprite != null) icon.sprite = lockedSprite;
                else if (manager != null)
                {
                    manager.GetDisplaySprite(data, out Sprite sprite);
                    icon.sprite = sprite;
                }
            }
        }

        // Name
        if (nameText != null)
            nameText.text = unlocked ? data.item_name : lockedName;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (manager == null || data == null) return;
        manager.ShowInfo(data);
    }
}