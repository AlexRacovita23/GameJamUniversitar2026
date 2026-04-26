using System;
using UnityEngine;

public class ObeliskController : MonoBehaviour
{
    [Header("Required Item")]
    [SerializeField] private ItemData requiredPotion;

    [Header("References")]
    [SerializeField] private TempleController templeController;

    public static event Action OnObeliskActivated;

    public bool TryActivate()
    {
        if (requiredPotion == null)
        {
            Debug.LogWarning("ObeliskController: No required potion assigned!");
            return false;
        }

        if (Inventory.Instance == null)
        {
            Debug.LogWarning("ObeliskController: Inventory not found!");
            return false;
        }

        int potionCount = Inventory.Instance.GetCount(requiredPotion);
        if (potionCount <= 0)
        {
            Debug.Log("ObeliskController: Player doesn't have the required potion.");
            return false;
        }

        Inventory.Instance.RemoveItem(requiredPotion);
        Debug.Log($"ObeliskController: Used {requiredPotion.ItemName}. Obelisk activated!");

        if (templeController != null)
        {
            templeController.isRising = true;
        }

        OnObeliskActivated?.Invoke();

        return true;
    }
}