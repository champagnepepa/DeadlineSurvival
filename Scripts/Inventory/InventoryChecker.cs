using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryChecker : MonoBehaviour
{
    public static InventoryChecker Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public bool HasItem(ItemSO item, int amount)
    {
        if (item == null)
        {
            Debug.LogWarning("⚠ HasItem dipanggil dengan item == null");
            return false;
        }

        int count = 0;

        if (InventoryManager.Instance == null)
        {
            Debug.LogError("❌ InventoryManager.Instance belum tersedia");
            return false;
        }

        foreach (var slot in InventoryManager.Instance.Slots)
        {
            InventorySlot s = slot.GetComponent<InventorySlot>();
            if (s.heldItem != null)
            {
                InventoryItem invItem = s.heldItem.GetComponent<InventoryItem>();
                if (invItem != null && invItem.itemScriptableObject == item)
                {
                    count += invItem.stackCurrent;
                    if (count >= amount)
                        return true;
                }
            }
        }

        return false;
    }

    public void RemoveItem(ItemSO item, int amount)
    {
        if (InventoryManager.Instance == null || item == null || amount <= 0)
        {
            Debug.LogWarning("❌ Tidak bisa RemoveItem: parameter tidak valid");
            return;
        }

        int remaining = amount;

        foreach (var slot in InventoryManager.Instance.Slots)
        {
            InventorySlot s = slot.GetComponent<InventorySlot>();
            if (s.heldItem != null)
            {
                InventoryItem invItem = s.heldItem.GetComponent<InventoryItem>();

                if (invItem != null && invItem.itemScriptableObject == item)
                {
                    if (invItem.stackCurrent <= remaining)
                    {
                        remaining -= invItem.stackCurrent;
                        Destroy(s.heldItem);
                        s.heldItem = null;
                    }
                    else
                    {
                        invItem.stackCurrent -= remaining;
                        invItem.UpdateStackDisplay();
                        remaining = 0;
                        break;
                    }
                }
            }
        }
    }

    public int GetItemCount(ItemSO item)
    {
        if (item == null || InventoryManager.Instance == null) return 0;

        int count = 0;
        foreach (var slot in InventoryManager.Instance.Slots)
        {
            InventorySlot s = slot.GetComponent<InventorySlot>();
            if (s != null && s.heldItem != null)
            {
                InventoryItem invItem = s.heldItem.GetComponent<InventoryItem>();
                if (invItem != null && invItem.itemScriptableObject == item)
                {
                    count += invItem.stackCurrent;
                }
            }
        }
        return count;
    }
}