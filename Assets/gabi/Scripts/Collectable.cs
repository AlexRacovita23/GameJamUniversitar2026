using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{ 
    public List<ItemData> itemList;

    public void CollectItem()
    {
        Debug.Log("Collecting item: " + gameObject.name);
        foreach (ItemData itemData in itemList)
            Inventory.Instance.AddItem(itemData);
        // Add any additional logic for collecting the item here (e.g., updating score, playing sound, etc.)
        Destroy(gameObject);
    }
}
