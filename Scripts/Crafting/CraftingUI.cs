using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingUI : MonoBehaviour
{
    [Header("Akses Crafting")]
    public Button craftingButton;
    public ItemSO requiredItem; // Multitool
    public GameObject craftingPanel;
    public GameObject warningPanel;
    public Button closeButton;

    [Header("Referensi UI")]
    public TMP_Dropdown recipeDropdown;
    public Button craftButton;
    public Transform ingredientsContainer;
    public GameObject ingredientSlotPrefab;
    public Image resultIcon;
    public TMP_Text resultName;
    public TMP_Text statsItems;

    [Header("Data")]
    public CraftingRecipeSO[] availableRecipes;
    private CraftingRecipeSO selectedRecipe;

    [Header("Audio")]
    [SerializeField] private AudioClip craftSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        craftingPanel.SetActive(false);
        if (warningPanel != null) warningPanel.SetActive(false);

        if (craftingButton != null)
            craftingButton.onClick.AddListener(TryOpenCrafting);

        PopulateRecipeDropdown();
        recipeDropdown.onValueChanged.AddListener(OnRecipeSelected);
        craftButton.onClick.AddListener(OnCraftButtonClicked);

        if (availableRecipes.Length > 0)
        {
            selectedRecipe = availableRecipes[0];
            UpdateRecipeDisplay();
        }
    }

    void PopulateRecipeDropdown()
    {
        recipeDropdown.ClearOptions();

        foreach (var recipe in availableRecipes)
        {
            recipeDropdown.options.Add(new TMP_Dropdown.OptionData(recipe.result.name));
        }

        recipeDropdown.RefreshShownValue();
    }

    void OnRecipeSelected(int index)
    {
        if (index < 0 || index >= availableRecipes.Length)
        {
            Debug.LogError("❌ Index resep tidak valid");
            return;
        }

        selectedRecipe = availableRecipes[index];
        UpdateRecipeDisplay();
    }

    void UpdateRecipeDisplay()
    {
        if (selectedRecipe == null)
        {
            Debug.LogWarning("❌ selectedRecipe == null");
            return;
        }

        foreach (Transform child in ingredientsContainer)
        {
            Destroy(child.gameObject);
        }

        bool allIngredientsMet = true;

        foreach (var ingredient in selectedRecipe.ingredients)
        {
            GameObject slot = Instantiate(ingredientSlotPrefab, ingredientsContainer);

            TMP_Text label = slot.GetComponentInChildren<TMP_Text>();
            Image iconImage = slot.GetComponentInChildren<Image>();

            int currentAmount = InventoryChecker.Instance.GetItemCount(ingredient.item);
            bool isEnough = currentAmount >= ingredient.amount;

            if (!isEnough) allIngredientsMet = false;

            if (label != null)
            {
                string colorHex = isEnough ? "#000000" : "#FF0000";

                label.text = $"<color={colorHex}>{ingredient.item.name} x{ingredient.amount}</color>";
            }

            if (iconImage != null)
                iconImage.sprite = ingredient.item.Icon;
        }

        resultName.text = selectedRecipe.result.name;
        resultIcon.sprite = selectedRecipe.result.Icon;
        statsItems.text = selectedRecipe.result.description;

        // 🔐 Cek jika player punya multitool & bahan
        bool hasMultitool = InventoryChecker.Instance != null && InventoryChecker.Instance.HasItem(requiredItem, 1);
        bool canCraft = CraftingManager.Instance != null && CraftingManager.Instance.CanCraft(selectedRecipe);

        craftButton.interactable = hasMultitool && canCraft;
    }

    void OnCraftButtonClicked()
    {
        if (selectedRecipe == null || CraftingManager.Instance == null)
        {
            Debug.LogWarning("❌ Tidak bisa craft. Resep null atau CraftingManager tidak tersedia.");
            return;
        }

        if (CraftingManager.Instance.CanCraft(selectedRecipe))
        {
            if (craftSound != null)
            {
                audioSource.PlayOneShot(craftSound);
            }

            CraftingManager.Instance.Craft(selectedRecipe);
            UpdateRecipeDisplay();
        }
    }

    public void TryOpenCrafting()
    {
        if (InventoryChecker.Instance == null)
        {
            Debug.LogError("❌ InventoryChecker.Instance == null");
            return;
        }

        if (requiredItem == null)
        {
            Debug.LogError("❌ requiredItem belum diassign di Inspector");
            return;
        }

        if (InventoryChecker.Instance.HasItem(requiredItem, 1))
        {
            craftingPanel.SetActive(true);
            if (warningPanel != null) warningPanel.SetActive(false);
            UpdateRecipeDisplay();
        }
        else
        {
            Debug.LogWarning("❌ Tidak punya Multitool");
            if (warningPanel != null)
            {
                warningPanel.SetActive(true);
                StopAllCoroutines();
                StartCoroutine(HideWarningAfterSeconds(2f));
            }
        }
    }

    private IEnumerator HideWarningAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (warningPanel != null)
            warningPanel.SetActive(false);
    }
}