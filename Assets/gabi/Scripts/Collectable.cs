using UnityEngine;

public class Collectable : MonoBehaviour
{
    [Header("Items to Add")]
    [SerializeField] private CollectableEntry[] itemsToCollect;

    [System.Serializable]
    public class CollectableEntry
    {
        public ItemData item;
        public int quantity = 1;
    }

    public void CollectItem()
    {
        if (Inventory.Instance == null)
        {
            Debug.LogError("[Collectable] Inventory.Instance is null! Make sure Inventory exists in the scene.");
            return;
        }

        foreach (var entry in itemsToCollect)
        {
            if (entry.item == null)
            {
                Debug.LogWarning("[Collectable] Null item in collectable entry, skipping.");
                continue;
            }

            for (int i = 0; i < entry.quantity; i++)
            {
                Inventory.Instance.AddItem(entry.item);
            }

            Debug.Log($"[Collectable] Collected {entry.quantity}x {entry.item.ItemName}");
        }

        PlayCollectEffects();
        Destroy(gameObject);
    }

    private void PlayCollectEffects()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayUIClick("Click");
    }
}