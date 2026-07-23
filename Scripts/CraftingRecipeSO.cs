using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Crafting/Recipe")]
public class CraftingRecipeSO : ScriptableObject
{
    [System.Serializable]
    public struct Ingredient
    {
        public ItemSO item;
        public int amount;
    }

    public Ingredient[] ingredients;
    public ItemSO result;

    private void OnValidate()
    {
        for (int i = 0; i < ingredients.Length; i++)
        {
            if (ingredients[i].item == null)
            {
                Debug.LogWarning($"[CraftingRecipeSO] Ingredient {i} belum diisi di {name}");
            }
        }

        if (result == null)
        {
            Debug.LogWarning($"[CraftingRecipeSO] Hasil crafting belum diisi di {name}");
        }
    }
}
