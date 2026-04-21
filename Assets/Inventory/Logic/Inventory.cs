using System.Collections.Generic;
public class Inventory
{
    private List<ItemData> _items = new();

    public IReadOnlyList<ItemData> Items => _items;

    public void AddItem(ItemData item) => _items.Add(item);
    public bool RemoveItem(ItemData item) => _items.Remove(item);
}