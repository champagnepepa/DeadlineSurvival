using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInfoDisplayUI : MonoBehaviour
{
    public static ItemInfoDisplayUI Instance;

    public GameObject panel;
    public Image icon;
    public TMP_Text itemName;
    public TMP_Text description;

    private InventorySlot currentSlot;

    public RectTransform panelRect;

    public Button equipButton;

    void Awake()
    {
        Debug.Log("🟢 ItemInfoDisplayUI aktif!");
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // pastikan hanya 1
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 🚀 Tidak akan terhapus saat pindah scene
        panel.SetActive(false);
    }

    void Update()
    {
        if (panel.activeSelf && Input.GetMouseButtonDown(0)) // klik kiri
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(panelRect, Input.mousePosition, null))
            {
                HideInfo(); // tutup panel
            }
        }
    }

    public void ShowInfo(ItemSO item, InventorySlot slotRef)
    {
        icon.sprite = item.Icon;
        itemName.text = item.name;
        description.text = item.description;

        currentSlot = slotRef; // simpan referensinya

        equipButton.gameObject.SetActive(item.itemType == ItemType.Weapon);
        panel.SetActive(true);
    }

    public void TestClick()
    {
        Debug.Log("Button clicked!");
    }

    public void OnConsumeButtonPressed()
    {
        GameObject itemObj = InventoryManager.Instance.selectedItemObject;
        if (itemObj == null) return;

        InventoryItem item = itemObj.GetComponent<InventoryItem>();
        if (item.itemScriptableObject.itemType == ItemType.Consumable)
        {
            item.itemScriptableObject.Consume();
            Destroy(itemObj);
            InventoryManager.Instance.selectedItemObject = null;
            HideInfo();
        }
    }

    public void OnDropButtonPressed()
    {
        GameObject itemObj = InventoryManager.Instance.selectedItemObject;
        if (itemObj == null) return;

        InventoryItem item = itemObj.GetComponent<InventoryItem>();
        if (item == null) return;

        ItemSO itemSO = item.itemScriptableObject;

        if (itemSO == null || itemSO.pickupPrefab == null)
        {
            Debug.LogWarning("⚠️ ItemSO atau pickupPrefab kosong. Tidak bisa drop.");
            return;
        }

        // Tentukan posisi drop di depan kamera
        Vector3 dropPosition = InventoryManager.Instance.cam.transform.position + InventoryManager.Instance.cam.transform.forward * 2f;

        // Spawn prefab world (pickupPrefab)
        GameObject dropped = GameObject.Instantiate(itemSO.pickupPrefab, dropPosition, Quaternion.identity);

        // Assign data jika ada komponen ItemPickable
        ItemPickable pickable = dropped.GetComponent<ItemPickable>();
        if (pickable != null)
        {
            pickable.itemScriptableObject = itemSO;
        }

        // Hapus dari inventory
        Destroy(itemObj);
        InventoryManager.Instance.selectedItemObject = null;

        for (int i = 0; i < InventoryManager.Instance.handParent.childCount; i++)
        {
            GameObject hand = InventoryManager.Instance.handParent.GetChild(i).gameObject;
            if (!hand.activeSelf) continue;

            ItemHand handItem = hand.GetComponent<ItemHand>();
            if (handItem != null && handItem.itemScriptableObject == item.itemScriptableObject)
            {
                hand.SetActive(false);
                Debug.Log("🧤 Item tangan dinonaktifkan: " + hand.name);
                break;
            }
        }

        // Sembunyikan panel info
        HideInfo();

        Debug.Log($"✅ {itemSO.name} berhasil di-drop ke dunia.");
    }

    public void OnEquipButtonPressed()
    {
        GameObject itemObj = InventoryManager.Instance.selectedItemObject;
        if (itemObj == null) return;

        InventoryItem item = itemObj.GetComponent<InventoryItem>();
        if (item.itemScriptableObject.itemType == ItemType.Weapon)
        {
            InventoryManager.Instance.EquipToHotbar(itemObj);
            InventoryManager.Instance.selectedItemObject = null;
            HideInfo();
        }
    }



    public void ShowInfo(ItemSO item)
    {
        icon.sprite = item.Icon;
        itemName.text = item.name;
        description.text = item.description;
        panel.SetActive(true);
    }

    public void HideInfo()
    {
        panel.SetActive(false);
        InventoryManager.Instance.selectedItemObject = null;
        currentSlot = null;
    }
}
