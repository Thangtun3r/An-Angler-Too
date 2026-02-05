using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIFishSlot : MonoBehaviour, IPointerClickHandler
{
    private string fishName;
    public Image fishImage;
    public FishInventory fishInventory;
    private int slotIndex;
    private ItemSO currentFish;

    public bool isDiscardSlot;
    public static int selectedIndex = -1;

    public void Initialize(int index, FishInventory fishInven)
    {
        slotIndex = index;
        fishInventory = fishInven;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (selectedIndex == -1)
        {
            if (!isDiscardSlot && fishInventory.GetItem(slotIndex) != null)
            {
                selectedIndex = slotIndex;
                CursorFollower.Instance?.SetIcon(fishImage.sprite);
                fishImage.color = new Color(1, 1, 1, 0);
            }
        }
        else
        {
            if (isDiscardSlot)
            {
                var selectedItem = fishInventory.GetItem(selectedIndex);
                if (selectedItem == null) return;          
                if (selectedItem.isQuestItem) return;        
                fishInventory.RemoveAt(selectedIndex);
            }
            else
            {
                fishInventory.SwapSlots(selectedIndex, slotIndex);
            }
    
            selectedIndex = -1;
            CursorFollower.Instance?.SetIcon(null);
        }
    }

    public void RefreshSlot()
    {
        if (isDiscardSlot) return;

        currentFish = fishInventory.GetItem(slotIndex);
        fishImage.color = Color.white;
        if (currentFish == null)
        {
            fishName = "Empty";
            fishImage.sprite = null;
            fishImage.color = new Color(1, 1, 1, 0);
            return;
        }
        fishName = currentFish.item_name;
        fishImage.sprite = currentFish.item_sprite;
    }
}