using UnityEngine;

public enum CollectableType
{
    InventoryItem,
    JournalPage
}

public class Collectable : MonoBehaviour
{
    [SerializeField] private CollectableType collectableType = CollectableType.InventoryItem;
    [SerializeField] private CollectableEntry[] itemsToCollect;

    [SerializeField] private GameObject journalPageToEnable;

    [System.Serializable]
    public class CollectableEntry
    {
        public ItemData item;
        public int quantity = 1;
    }

    public void CollectItem()
    {
        switch (collectableType)
        {
            case CollectableType.InventoryItem:
                CollectInventoryItems();
                break;

            case CollectableType.JournalPage:
                CollectJournalPage();
                break;
        }

        PlayCollectEffects();
        Destroy(gameObject);
    }

    private void CollectInventoryItems()
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
    }

    private void CollectJournalPage()
    {
        if (journalPageToEnable == null)
        {
            Debug.LogWarning("[Collectable] No journal page assigned to enable!");
            return;
        }

        journalPageToEnable.SetActive(true);
        Debug.Log($"[Collectable] Journal page '{journalPageToEnable.name}' unlocked!");
    }

    private void PlayCollectEffects()
    {
        if (AudioManager.Instance != null)
        {
            switch (collectableType)
            {
                case CollectableType.InventoryItem:
                    AudioManager.Instance.PlayUIClick("Click");
                    break;

                case CollectableType.JournalPage:
                    AudioManager.Instance.PlayUIClick("Writing");
                    break;
            }
        }
    }
}