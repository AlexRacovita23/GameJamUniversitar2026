using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    private Dictionary<ItemData, int> _items = new();
    public IReadOnlyDictionary<ItemData, int> Items => _items;

    public event Action OnInventoryChanged;

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

    public void AddItem(ItemData item)
    {
        if (_items.TryGetValue(item, out int current))
            _items[item] = current + 1;
        else
            _items[item] = 1;

        OnInventoryChanged?.Invoke();
    }

    public bool RemoveItem(ItemData item)
    {
        if (!_items.TryGetValue(item, out int current)) return false;
        if (current <= 1)
            _items.Remove(item);
        else
            _items[item] = current - 1;

        OnInventoryChanged?.Invoke();
        return true;
    }

    public int GetCount(ItemData item)
    {
        _items.TryGetValue(item, out int count);
        return count;
    }
}