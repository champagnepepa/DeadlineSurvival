using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDurability : MonoBehaviour
{
    public int maxDurability;
    public int currentDurability;
    public GameObject weaponObject;

    private void Start()
    {
        ResetDurability();
    }

    public void OnEnable()
    {
        ResetDurability();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Hit enemy!");
            currentDurability--;

            if (currentDurability <= 0)
            {
                BreakWeapon();
            }
        }
    }

    private void BreakWeapon()
    {
        InventoryManager inventoryManager = InventoryManager.Instance;
        if (inventoryManager != null)
        {
            inventoryManager.RemoveBrokenWeaponFromHotbar(gameObject);
        }

        gameObject.SetActive(false);
    }

    public void ResetDurability()
    {
        currentDurability = maxDurability;
        Debug.Log(gameObject.name + " Durability di-reset ke: " + currentDurability);
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Enemy"))
    //    {
    //        Debug.Log("Hit enemy!");
    //        durability--;
    //
    //        if (durability <= 0)
    //        {
    //            Debug.Log("Weapon broke!");
    //
    //            // Coba hapus dari hotbar
    //            InventoryManager inventoryManager = InventoryManager.Instance;
    //            if (inventoryManager != null)
    //            {
    //                inventoryManager.RemoveBrokenWeaponFromHotbar(gameObject);
    //            }
    //
    //            Destroy(gameObject);
    //        }
    //    }
    //}

}
