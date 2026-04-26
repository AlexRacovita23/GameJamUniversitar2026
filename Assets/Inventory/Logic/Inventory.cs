using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    private Dictionary<ItemData, int> _items = new();
    public IReadOnlyDictionary<ItemData, int> Items => _items;

    public event Action<ItemData, int> OnItemAdded;
    public event Action<ItemData, int> OnItemRemoved;
    public event Action OnInventoryChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // DontDestroyOnLoad(gameObject);
    }

    public void AddItem(ItemData item)
    {
        if (_items.TryGetValue(item, out int current))
            _items[item] = current + 1;
        else
            _items[item] = 1;

        int newCount = _items[item];

        OnItemAdded?.Invoke(item, newCount);
        OnInventoryChanged?.Invoke();
    }

    public bool RemoveItem(ItemData item)
    {
        if (!_items.TryGetValue(item, out int current)) return false;

        int remainingCount;
        if (current <= 1)
        {
            _items.Remove(item);
            remainingCount = 0;
        }
        else
        {
            _items[item] = current - 1;
            remainingCount = _items[item];
        }

        OnItemRemoved?.Invoke(item, remainingCount);
        OnInventoryChanged?.Invoke();
        return true;
    }

    public int GetCount(ItemData item)
    {
        _items.TryGetValue(item, out int count);
        return count;
    }
}