using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Image iconImage;
    [SerializeField] Text stackText;
    [SerializeField] GameObject newBadge;

    [Header("Item Data")]
    public ItemSO itemScriptableObject;

    public int stackCurrent = 1;
    public int stackMax;

    public bool isNewItem = false;

    private void Start()
    {
        stackMax = itemScriptableObject.stackMax;

        if (itemScriptableObject.Icon != null)
        {
            iconImage.sprite = itemScriptableObject.Icon;
            Debug.Log("✅ Icon ditemukan dan ditampilkan: " + itemScriptableObject.name);
        }
        else
        {
            Debug.LogWarning("❌ Icon kosong di ItemSO: " + itemScriptableObject.name);
        }

        UpdateStackDisplay();
        UpdateNewBadge();
        //  itemInfoUI = InventoryManager.Instance.ItemInfoUi;
        //  itemInfoUI_itemName = itemInfoUI.transform.Find("itemName").GetComponent<Text>/();
        //  itemInfoUI_itemDescription = itemInfoUI.transform.Find//("itemDescription").GetComponent<Text>();
        //  itemInfoUI_itemFunctionality = itemInfoUI.transform.Find("itemFunctionality").GetComponent<Text>();

        if (itemScriptableObject.associatedKey != null)
        {
            KeyInventory.Instance.AddKey(itemScriptableObject.associatedKey);
            Debug.Log("🔑 Kunci '" + itemScriptableObject.associatedKey.keyName + "' ditambahkan ke KeyInventory dari slot item.");
        }
    }

    public void UpdateStackDisplay()
    {
        if (stackMax > 1)
        {
            stackText.text = stackCurrent.ToString();
            stackText.enabled = true;
        }
        else
        {
            stackText.enabled = false;
        }
    }

    public void SetNew(bool value)
    {
        isNewItem = value;

        if (newBadge != null)
            newBadge.SetActive(value);

        UpdateNewBadge();
    }

    void UpdateNewBadge()
    {
        if (newBadge != null)
            newBadge.SetActive(isNewItem);
    }

    public void MarkAsChecked()
    {
        if (!isNewItem) return;

        isNewItem = false;
        newBadge.SetActive(false);

        InventoryManager.Instance.RegisterDiscoveredItem(itemScriptableObject.itemID);
    }
}
