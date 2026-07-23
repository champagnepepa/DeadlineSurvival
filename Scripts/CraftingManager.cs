using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    [Header("Syarat Akses Crafting")]
    public ItemSO requiredItem; // Multitool sebagai syarat crafting

    public static CraftingManager Instance;
    public CraftingRecipeSO[] recipes;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool CanAccessCrafting()
    {
        if (requiredItem == null)
        {
            Debug.LogWarning("requiredItem belum di-assign di Inspector (CraftingManager)!");
            return true; // fallback: boleh crafting kalau belum di-set
        }

        if (InventoryChecker.Instance == null)
        {
            Debug.LogError("InventoryChecker.Instance == null saat CanAccessCrafting()");
            return false;
        }

        return InventoryChecker.Instance.HasItem(requiredItem, 1);
    }

    public bool CanCraft(CraftingRecipeSO recipe)
    {
        if (recipe == null)
        {
            Debug.LogWarning("CanCraft: recipe == null");
            return false;
        }

        if (requiredItem != null)
        {
            if (InventoryChecker.Instance == null)
            {
                Debug.LogError("InventoryChecker.Instance == null saat CanCraft()");
                return false;
            }

            if (!InventoryChecker.Instance.HasItem(requiredItem, 1))
            {
                Debug.LogWarning("Tidak punya Multitool. Tidak bisa crafting.");
                return false;
            }
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogError("InventoryManager.Instance == null saat CanCraft()");
            return false;
        }

        var inv = InventoryManager.Instance.GetCounts();

        foreach (var ing in recipe.ingredients)
        {
            if (!inv.ContainsKey(ing.item) || inv[ing.item] < ing.amount)
            {
                Debug.LogWarning($"Tidak cukup bahan: {ing.item.name}");
                return false;
            }
        }

        return true;
    }

    public void Craft(CraftingRecipeSO recipe)
    {
        if (!CanCraft(recipe))
        {
            Debug.LogWarning("Tidak bisa crafting. Syarat tidak terpenuhi.");
            return;
        }

        Debug.Log("Crafting dimulai...");

        foreach (var ing in recipe.ingredients)
        {
            Debug.Log($"➖ Mengurangi: {ing.item.name} x{ing.amount}");
            InventoryManager.Instance.Remove(ing.item, ing.amount);
        }

        Debug.Log($"➕ Menambahkan hasil: {recipe.result.name}");
        InventoryManager.Instance.Add(recipe.result, 1);
    }
}