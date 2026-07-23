using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public GameObject heldItem;
    public ItemSO itemData;
    public GameObject itemObject;

    public void SetHeldItem(GameObject item)
    {
        heldItem = item;

        // Ganti parent dengan false agar posisi lokal tetap konsisten
        heldItem.transform.SetParent(transform, false);

        // Reset posisi dan skala
        RectTransform rt = heldItem.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchoredPosition = Vector2.zero;
            rt.localScale = Vector3.one;
        }
        else
        {
            heldItem.transform.localPosition = Vector3.zero;
            heldItem.transform.localScale = Vector3.one;
        }

        // ⛑ Tambahkan ini untuk memaksa Canvas update layout dan render
        Canvas.ForceUpdateCanvases();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (heldItem == null) return;

        var item = heldItem.GetComponent<InventoryItem>().itemScriptableObject;
        ItemInfoDisplayUI.Instance.ShowInfo(item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ItemInfoDisplayUI.Instance.HideInfo();
    }
}
