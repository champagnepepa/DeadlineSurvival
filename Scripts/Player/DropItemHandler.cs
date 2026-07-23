using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropItemHandler : MonoBehaviour, IDropHandler
{
    GameObject draggedObject;
    GameObject lastItemSlot;
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;

        GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;

        InventorySlot slot = clickedObject.GetComponent<InventorySlot>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        GameObject droppedItem = Instantiate(draggedObject.GetComponent<InventoryItem>().itemScriptableObject.prefab, player.transform.position, Quaternion.identity);
        //droppedItem.GetComponent<ItemObject>().amount = slot.AmountInSlot;
        draggedObject = null;
        //slot.AmountInSlot = 0;
    }
}