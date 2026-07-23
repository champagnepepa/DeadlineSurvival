using UnityEngine;

public class ItemPickable : MonoBehaviour
{
    public string itemID; // Untuk penanda unik
    public ItemSO itemScriptableObject; // ← Tambahkan ini agar tidak error

    void Start()
    {
        if (GameStateManager.Instance.IsItemPicked(itemID))
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameStateManager.Instance.MarkItemPicked(itemID);

            // Tambahkan ke inventory
            InventoryManager.Instance.ItemPicked(gameObject);

            Destroy(gameObject);
        }
    }
}