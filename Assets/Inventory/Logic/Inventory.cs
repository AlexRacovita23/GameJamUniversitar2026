using System.Collections.Generic;

public class Inventory
{
    private Dictionary<ItemData, int> _items = new();
    public IReadOnlyDictionary<ItemData, int> Items => _items;

    public void AddItem(ItemData item)
    {
        if (_items.TryGetValue(item, out int current))
            _items[item] = current + 1;
        else
            _items[item] = 1;
    }

    public bool RemoveItem(ItemData item)
    {
        if (!_items.TryGetValue(item, out int current)) return false;

        if (current <= 1)
            _items.Remove(item);
        else
            _items[item] = current - 1;

        return true;
    }

    public int GetCount(ItemData item)
    {
        _items.TryGetValue(item, out int count);
        return count;
    }
}