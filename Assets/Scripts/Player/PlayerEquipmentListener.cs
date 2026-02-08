using System;
using UnityEngine;

public class PlayerEquipmentListener : MonoBehaviour
{
    [Serializable]
    public class EquipmentRule
    {
        public ItemSO item;

        [Tooltip("Any MonoBehaviour that implements IEquipment")]
        public MonoBehaviour[] equipmentBehaviours;

        public GameObject[] enableObjects;
    }

    [SerializeField] private FishInventory inventory;
    [SerializeField] private EquipmentRule[] rules;

    private void Start()
    {
        ApplyAllRules();
    }

    private void OnEnable()
    {
        FishInventory.OnInventoryChanged += ApplyAllRules;
    }

    private void OnDisable()
    {
        FishInventory.OnInventoryChanged -= ApplyAllRules;
    }

    private void ApplyAllRules()
    {
        if (inventory == null || rules == null) return;

        foreach (var rule in rules)
        {
            if (rule == null || rule.item == null) continue;

            bool equipped = inventory.Contains(rule.item);

            // 🔑 Generic equipment handling
            if (rule.equipmentBehaviours != null)
            {
                foreach (var mb in rule.equipmentBehaviours)
                {
                    if (mb is IEquipment equipment)
                    {
                        equipment.SetEquipped(equipped);
                    }
                }
            }

            // Optional GameObjects
            if (rule.enableObjects != null)
            {
                foreach (var go in rule.enableObjects)
                {
                    if (go != null)
                        go.SetActive(equipped);
                }
            }
        }
    }
}