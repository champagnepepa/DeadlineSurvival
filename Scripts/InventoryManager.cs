using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
//using static UnityEditor.Progress;

public class InventoryManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] public GameObject ClickedItemUI;

    public static InventoryManager Instance { get; set; }

    [SerializeField] public GameObject[] hotbarSlots = new GameObject[3];
    [SerializeField] private GameObject[] slots = new GameObject[20];

    [SerializeField] private List<string> discoveredItemIDs = new List<string>();

    [SerializeField] private Transform uiItemHolder;

    public GameObject[] Slots => slots;
    [SerializeField] GameObject inventoryParent;
    [SerializeField] public Transform handParent;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] public Camera cam;

    [SerializeField] private RectTransform inventoryRect;
    [SerializeField] private float slideDuration = 0.3f;
    [SerializeField] private Vector2 visiblePosition = new Vector2(18, 92);  // posisi saat terlihat
    [SerializeField] private Vector2 hiddenPosition = new Vector2(18, -987); // posisi sembunyi di bawah
    private Coroutine slideCoroutine;

    [SerializeField] private GameObject overlayPanel;
    [SerializeField] private GameObject isInventoryParent;

    [Header("Audio")]
    [SerializeField] private AudioClip popupSound;
    //[SerializeField] private AudioClip closeSound;
    private AudioSource audioSource;

    private bool isInventoryOpened = false;

    GameObject draggedObject;
    GameObject lastItemSlot;
    GameObject gameObject;

    private float healthEffect;
    private float caloriesEffect;
    private float hydrationEffect;

    int selectedHotbarSlot = 0;

    public PlayerMovement pm;

    public ItemSO itemData;

    public GameObject selectedItemObject;


    private HashSet<string> discoveredItems = new HashSet<string>();

    // InventorySlot selectedItemSlot;

    public void RegisterDiscoveredItem(string itemID)
    {
        if (!discoveredItems.Contains(itemID))
            discoveredItems.Add(itemID);
    }

    public bool IsItemDiscovered(string itemID)
    {
        return discoveredItems.Contains(itemID);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        //DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (cam == null)
            cam = Camera.main;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // pastikan audio 2D
        audioSource.volume = 1f;

        HotbarItemChanged();

        Debug.Log("✅ InventoryManager started.");
    }

    void Update()
    {
        CheckForHotbarInput();

        if (draggedObject != null)
        {
            draggedObject.transform.position = Input.mousePosition;
        }

        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    if (isInventoryOpened)
        //    {
        //        pm.isInterrupted = false;
        //        //Cursor.lockState = CursorLockMode.Locked;
        //        isInventoryOpened = false;
        //    }
        //    else
        //    {
        //        pm.isInterrupted = true;
        //        //Cursor.lockState = CursorLockMode.None;
        //        isInventoryOpened = true;
        //    }
        //}

        if (Input.GetKeyDown(KeyCode.F))
        {
            isInventoryOpened = !isInventoryOpened;

            if (slideCoroutine != null)
                StopCoroutine(slideCoroutine);

            if (isInventoryOpened)
            {
                pm.isInterrupted = true;
                inventoryParent.SetActive(true);
                overlayPanel.SetActive(true); // tambahkan overlay juga
                slideCoroutine = StartCoroutine(SlideInventory(inventoryRect, hiddenPosition, visiblePosition));
                if (popupSound != null) 
                    audioSource.PlayOneShot(popupSound);
                else
                    Debug.LogWarning("popupSound belum di-assign!");
            }
            else
            {
                pm.isInterrupted = false;
                overlayPanel.SetActive(false); // hilangkan overlay juga
                slideCoroutine = StartCoroutine(SlideInventory(inventoryRect, visiblePosition, hiddenPosition, () =>
                {
                    inventoryParent.SetActive(false);
                }));
            }
        }
        else if (isInventoryOpened && Input.GetKeyDown(KeyCode.Escape))
        {
            isInventoryOpened = false;
            pm.isInterrupted = false;

            if (slideCoroutine != null)
                StopCoroutine(slideCoroutine);

            overlayPanel.SetActive(false);

            slideCoroutine = StartCoroutine(SlideInventory(inventoryRect, visiblePosition, hiddenPosition, () =>
            {
                inventoryParent.SetActive(false);
            }));
        }

    }

    void ToggleInventory()
    {
        isInventoryOpened = !isInventoryOpened;

        overlayPanel.SetActive(isInventoryOpened);
        inventoryParent.SetActive(isInventoryOpened);
    }

    IEnumerator SlideInventory(RectTransform panel, Vector2 from, Vector2 to, System.Action onComplete = null)
    {
        float elapsed = 0f;

        panel.anchoredPosition = from;

        while (elapsed < slideDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / slideDuration);
            panel.anchoredPosition = Vector2.Lerp(from, to, t);
            yield return null;
        }

        panel.anchoredPosition = to;

        onComplete?.Invoke();
    }

    private void CheckForHotbarInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedHotbarSlot = 0;
            HotbarItemChanged();

            PlayerMovement.Instance.Anim.SetBool("IsEquip", true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedHotbarSlot = 1;
            HotbarItemChanged();
        }
    }

    private void HotbarItemChanged()
    {
        bool foundActiveHandItem = false;

        for (int i = 0; i < handParent.childCount; i++)
            handParent.GetChild(i).gameObject.SetActive(false);

        foreach (GameObject slot in hotbarSlots)
        {
            Vector3 scale;

            if (slot == hotbarSlots[selectedHotbarSlot])
            {
                scale = new Vector3(1.1f, 1.1f, 1.1f);

                if (slot.GetComponent<InventorySlot>().heldItem != null)
                {
                    for (int i = 0; i < handParent.childCount; i++)
                    {
                        var handItem = handParent.GetChild(i);
                        if (handItem.GetComponent<ItemHand>().itemScriptableObject ==
                            slot.GetComponent<InventorySlot>().heldItem.GetComponent<InventoryItem>().itemScriptableObject)
                        {
                            handItem.gameObject.SetActive(true);
                            foundActiveHandItem = true;

                            WeaponDurability durabilityScript = handItem.GetComponent<WeaponDurability>();
                            if (durabilityScript != null)
                            {
                                durabilityScript.ResetDurability();
                            }
                        }
                    }
                }
            }
            else
            {
                scale = new Vector3(0.9f, 0.9f, 0.9f);
            }

            slot.transform.localScale = scale;
        }

        // Update Animator parameter secara real-time
        PlayerMovement.Instance.Anim.SetBool("IsEquip", foundActiveHandItem);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
        if (clickedObject == null) return;

        InventorySlot slot = clickedObject.GetComponent<InventorySlot>();

        // Klik kiri = drag & tampilkan info
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (slot != null && slot.heldItem != null)
            {
                // 🔁 Kembalikan drag
                draggedObject = slot.heldItem;
                slot.heldItem = null;
                lastItemSlot = clickedObject;
            }
        }

        // Klik kanan = konsumsi
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (slot != null && slot.heldItem != null)
            {
                InventoryItem item = slot.heldItem.GetComponent<InventoryItem>();
                if (item != null)
                {
                    // Tampilkan info panel
                    ItemInfoDisplayUI.Instance.ShowInfo(item.itemScriptableObject);
                    item.MarkAsChecked();
                    // Simpan item yang dipilih agar bisa dikonsumsi/didrop
                    selectedItemObject = slot.heldItem;

                    item.MarkAsChecked();
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (draggedObject != null && eventData.pointerCurrentRaycast.gameObject != null && eventData.button == PointerEventData.InputButton.Left)
        {
            GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
            InventorySlot slot = clickedObject.GetComponent<InventorySlot>();

            if (slot != null && slot.heldItem == null)
            {
                slot.SetHeldItem(draggedObject);
                Canvas.ForceUpdateCanvases();
                draggedObject = null;
            }
            else if (slot != null && slot.heldItem != null && slot.heldItem.GetComponent<InventoryItem>().stackCurrent == slot.heldItem.GetComponent<InventoryItem>().stackMax || slot != null && slot.heldItem != null && slot.heldItem.GetComponent<InventoryItem>().itemScriptableObject != draggedObject.GetComponent<InventoryItem>().itemScriptableObject)
            {
                lastItemSlot.GetComponent<InventorySlot>().SetHeldItem(slot.heldItem);
                slot.heldItem.transform.SetParent(slot.transform.parent.parent.GetChild(2));
                slot.SetHeldItem(draggedObject);
                draggedObject.transform.SetParent(slot.transform.parent.parent.GetChild(2));
            }

            else if (slot != null && slot.heldItem != null && slot.heldItem.GetComponent<InventoryItem>().stackCurrent < slot.heldItem.GetComponent<InventoryItem>().stackMax
                && slot.heldItem.GetComponent<InventoryItem>().itemScriptableObject == draggedObject.GetComponent<InventoryItem>().itemScriptableObject)
            {
                InventoryItem slotHeldItem = slot.heldItem.GetComponent<InventoryItem>();
                InventoryItem draggedItem = draggedObject.GetComponent<InventoryItem>();

                int itemToFillStack = slotHeldItem.stackMax - slotHeldItem.stackCurrent;

                if (itemToFillStack >= draggedItem.stackCurrent)
                {
                    slotHeldItem.stackCurrent += draggedItem.stackCurrent;
                    DestroyImmediate(draggedObject);
                }
                else
                {
                    slotHeldItem.stackCurrent += itemToFillStack;
                    draggedItem.stackCurrent -= itemToFillStack;
                    lastItemSlot.GetComponent<InventorySlot>().SetHeldItem(draggedObject);
                }
            }
            else if (clickedObject.name != "DropItem")
            {
                lastItemSlot.GetComponent<InventorySlot>().SetHeldItem(draggedObject);
                draggedObject.transform.SetParent(slot.transform.parent.parent.GetChild(2));

            }
            else
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                Vector3 position = ray.GetPoint(3);

                GameObject newItem = Instantiate(draggedObject.GetComponent<InventoryItem>().itemScriptableObject.prefab, position, new Quaternion());
                newItem.GetComponent<ItemPickable>().itemScriptableObject = draggedObject.GetComponent<InventoryItem>().itemScriptableObject;

                lastItemSlot.GetComponent<InventorySlot>().heldItem = null;
                DestroyImmediate(draggedObject);
            }

            HotbarItemChanged();
            draggedObject = null;
        }

        //if (eventData.button == PointerEventData.InputButton.Right)
        //{
        //    GameObject clickedObject = //eventData.pointerCurrentRaycast.gameObject;
        //    InventorySlot slot = clickedObject.GetComponent<InventorySlot>/();
        //
        //    if (itemData != null && itemData.isConsumable)
        //    {
        //        
        //    }
        //}
    }

    public GameObject GetSelectedItem()
    {
        return selectedItemObject;
    }

    public List<InventoryItemData> GetItems()
    {
        List<InventoryItemData> items = new List<InventoryItemData>();
        foreach (var slot in slots)
        {
            var invSlot = slot.GetComponent<InventorySlot>();
            if (invSlot.heldItem != null)
            {
                InventoryItem ii = invSlot.heldItem.GetComponent<InventoryItem>();
                if (ii.itemScriptableObject != null)
                {
                    items.Add(new InventoryItemData(ii.itemScriptableObject.itemID, ii.stackCurrent));
                }
            }
        }
        return items;
    }

    public void SetItems(List<InventoryItemData> items)
    {
        ClearAllItems(); // penting agar tidak tumpuk

        foreach (var item in items)
        {
            ItemSO so = FindItemSOByID(item.itemID);
            if (so != null)
            {
                Add(so, item.amount);
            }
            else
            {
                Debug.LogWarning($"Item {item.itemID} not found!");
            }
        }
    }
    public void ClearAllItems()
    {
        foreach (var slot in slots)
        {
            var invSlot = slot.GetComponent<InventorySlot>();
            if (invSlot.heldItem != null)
            {
                Destroy(invSlot.heldItem);
                invSlot.heldItem = null;
            }
        }
    }

    public void ClearSelectedItem()
    {
        selectedItemObject = null;
    }

    public void RemoveItemObject(GameObject itemObject)
    {
        foreach (var slot in slots)
        {
            InventorySlot s = slot.GetComponent<InventorySlot>();
            if (s.heldItem == itemObject)
            {
                Destroy(itemObject);
                s.heldItem = null;
                return;
            }
        }
    }

    private void ClearAllNewItems()
    {
        foreach (var slotGO in slots)
        {
            InventorySlot slot = slotGO.GetComponent<InventorySlot>();
            if (slot.heldItem != null)
            {
                InventoryItem item = slot.heldItem.GetComponent<InventoryItem>();
                if (item.isNewItem)
                {
                    item.SetNew(false);
                    RegisterDiscoveredItem(item.itemScriptableObject.itemID);
                }
            }
        }
    }

    public void EquipToHotbar(GameObject itemObj)
    {
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            InventorySlot slot = hotbarSlots[i].GetComponent<InventorySlot>();
            if (slot.heldItem == null)
            {
                slot.SetHeldItem(itemObj);
                itemObj.transform.SetParent(uiItemHolder, false); // Parent UI item
                selectedItemObject = null;
                HotbarItemChanged();
                Debug.Log("✅ Item di-equip ke hotbar.");
                return;
            }
        }

        Debug.LogWarning("⚠️ Semua slot hotbar penuh.");
    }

    //public void UnequipHandItem(ItemSO itemToUnequip)
    //{
    //    for (int i = 0; i < handParent.childCount; i++)
    //    {
    //        var child = handParent.GetChild(i);
    //        var itemHand = child.GetComponent<ItemHand>();
    //
    //        if (itemHand != null && itemHand.itemScriptableObject == itemToUnequip)
    //        {
    //            child.gameObject.SetActive(false);
    //            Debug.Log("🔄 Senjata di tangan dinonaktifkan.");
    //            return;
    //        }
    //    }
    //}


    public void ItemPicked(GameObject pickedItem)
    {
        GameObject emptySlot = null;

        for (int i = 0; i < slots.Length; i++)
        {
            InventorySlot slot = slots[i].GetComponent<InventorySlot>();

            if (slot.heldItem == null)
            {
                emptySlot = slots[i];
                break;
            }
        }

        if (emptySlot != null)
        {
            GameObject newItem = Instantiate(itemPrefab);

            //  DEKLARASI invItem DI SINI
            InventoryItem invItem = newItem.GetComponent<InventoryItem>();

            invItem.itemScriptableObject =
                pickedItem.GetComponent<ItemPickable>().itemScriptableObject;

            bool isDiscovered = discoveredItemIDs.Contains(
            invItem.itemScriptableObject.itemID);

            invItem.SetNew(!isDiscovered);

            //  NEW ITEM CHECK
            //if (!IsItemDiscovered(invItem.itemScriptableObject.itemID))
            //{
            //    invItem.SetNew(true);
            //}

            newItem.transform.SetParent(emptySlot.transform.parent.parent.GetChild(2));
            invItem.stackCurrent = 1;

            emptySlot.GetComponent<InventorySlot>().SetHeldItem(newItem);
            newItem.transform.localScale = Vector3.one;

            Destroy(pickedItem);
        }
    }

    public Dictionary<ItemSO, int> GetCounts()
    {
        Dictionary<ItemSO, int> itemCounts = new Dictionary<ItemSO, int>();

        foreach (GameObject slotGO in slots)
        {
            InventorySlot slot = slotGO.GetComponent<InventorySlot>();
            if (slot.heldItem != null)
            {
                InventoryItem invItem = slot.heldItem.GetComponent<InventoryItem>();
                ItemSO item = invItem.itemScriptableObject;

                if (itemCounts.ContainsKey(item))
                    itemCounts[item] += invItem.stackCurrent;
                else
                    itemCounts[item] = invItem.stackCurrent;
            }
        }

        return itemCounts;
    }

    public void Remove(ItemSO itemToRemove, int amount)
    {
        for (int i = 0; i < slots.Length && amount > 0; i++)
        {
            InventorySlot slot = slots[i].GetComponent<InventorySlot>();
            if (slot.heldItem != null)
            {
                InventoryItem invItem = slot.heldItem.GetComponent<InventoryItem>();
                if (invItem.itemScriptableObject == itemToRemove)
                {
                    if (invItem.stackCurrent > amount)
                    {
                        invItem.stackCurrent -= amount;
                        invItem.UpdateStackDisplay();
                        return;
                    }
                    else
                    {
                        amount -= invItem.stackCurrent;
                        Destroy(slot.heldItem);
                        slot.heldItem = null;
                    }
                }
            }
        }
    }
    public void Add(ItemSO itemToAdd, int amount)
    {
        for (int i = 0; i < slots.Length && amount > 0; i++)
        {
            InventorySlot slot = slots[i].GetComponent<InventorySlot>();
            if (slot.heldItem != null)
            {
                InventoryItem invItem = slot.heldItem.GetComponent<InventoryItem>();
                if (invItem.itemScriptableObject == itemToAdd && invItem.stackCurrent < invItem.stackMax)
                {
                    int addable = Mathf.Min(invItem.stackMax - invItem.stackCurrent, amount);
                    invItem.stackCurrent += addable;
                    invItem.UpdateStackDisplay();
                    amount -= addable;
                }
            }
        }
        for (int i = 0; i < slots.Length && amount > 0; i++)
        {
            InventorySlot slot = slots[i].GetComponent<InventorySlot>();
            if (slot.heldItem == null)
            {
                GameObject newItem = Instantiate(itemPrefab);
                InventoryItem newInvItem = newItem.GetComponent<InventoryItem>();

                newInvItem.itemScriptableObject = itemToAdd;
                newInvItem.stackCurrent = amount;
                newInvItem.stackMax = itemToAdd.stackMax;

                bool isDiscovered = IsItemDiscovered(itemToAdd.itemID);
                newInvItem.SetNew(!isDiscovered);

                newItem.transform.SetParent(slot.transform.parent.parent.GetChild(2));
                newItem.transform.localScale = Vector3.one;
                slot.SetHeldItem(newItem);
                return;
            }
        }

        Debug.LogWarning("Inventory penuh! Item tidak bisa ditambahkan.");

    }

    public List<string> GetDiscoveredItemIDs()
    {
        return new List<string>(discoveredItems);
    }

    public void LoadDiscoveredItems(List<string> loadedIDs)
    {
        discoveredItems.Clear();

        if (loadedIDs == null) return;

        foreach (string id in loadedIDs)
            discoveredItems.Add(id);
    }

    public void RemoveBrokenWeaponFromHotbar(GameObject brokenWeapon)
    {
        ItemHand brokenItemHand = brokenWeapon.GetComponent<ItemHand>();
        if (brokenItemHand == null) return;

        ItemSO brokenItemSO = brokenItemHand.itemScriptableObject;

        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            InventorySlot slot = hotbarSlots[i].GetComponent<InventorySlot>();
            if (slot.heldItem != null)
            {
                InventoryItem invItem = slot.heldItem.GetComponent<InventoryItem>();
                if (invItem != null && invItem.itemScriptableObject == brokenItemSO)
                {
                    Destroy(slot.heldItem);
                    slot.heldItem = null;

                    // Hapus dari tangan juga
                    for (int j = 0; j < handParent.childCount; j++)
                    {
                        Transform handChild = handParent.GetChild(j);
                        ItemHand handItem = handParent.GetChild(j).GetComponent<ItemHand>();
                        if (handItem != null && handItem.itemScriptableObject == brokenItemSO)
                        {
                            handChild.gameObject.SetActive(false);
                            break;
                        }
                    }

                    HotbarItemChanged(); // update tampilan
                    break;
                }
            }
        }
    }

    private ItemSO FindItemSOByID(string id)
    {
        // misalnya semua ItemSO disimpan di folder Resources/Items
        var allItems = Resources.LoadAll<ItemSO>("Items");
        foreach (var item in allItems)
        {
            if (item.itemID == id)
                return item;
        }
        return null;
    }

}
