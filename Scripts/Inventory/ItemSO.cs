using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum ItemType
{
    Weapon,
    Consumable,
    Tool,
    Key,
    Battery
}

[CreateAssetMenu(fileName ="Item", menuName = "Scriptable Objects/Item")]
[System.Serializable]
public class ItemSO : ScriptableObject
{
    public GameObject previewPrefab;

    public string itemID;
    public string name;
    public Sprite Icon;
    public GameObject prefab;
    public int stackMax;

    [TextArea(3, 10)]
    public string description;

    [Header("Optional Key Data")]
    public Key associatedKey;

    public GameObject itemPendingConsumption;
    public bool isConsumable;

    public float healthEffect;
    public float caloriesEffect;
    public float hydrationEffect;

    public GameObject handPrefab;     // prefab di tangan player
    public GameObject pickupPrefab;   // prefab di dunia (yang bisa diambil)

    public ItemType itemType;

    public void Consume()
    {
        ApplyHealthEffect();
        ApplyCaloriesEffect();
        ApplyHydrationEffect();
    }

    private void ApplyHealthEffect()
    {
        float current = PlayerState.Instance.currentHealth;
        float max = PlayerState.Instance.maxHealth;

        if (healthEffect != 0)
        {
            PlayerState.Instance.setHealth(Mathf.Min(current + healthEffect, max));
        }
    }

    private void ApplyCaloriesEffect()
    {
        float current = PlayerState.Instance.currentHunger;
        float max = PlayerState.Instance.maxHunger;

        if (caloriesEffect != 0)
        {
            PlayerState.Instance.setCalories(Mathf.Min(current + caloriesEffect, max));
        }
    }

    private void ApplyHydrationEffect()
    {
        float current = PlayerState.Instance.currentThirst;
        float max = PlayerState.Instance.maxThirst;

        if (hydrationEffect != 0)
        {
            PlayerState.Instance.setHydration(Mathf.Min(current + hydrationEffect, max));
        }
    }
}
