using System.Collections.Generic;

public class CraftingTable
{
    private List<ItemData> _slots = new();
    public int MaxSlots = 4;

    public IReadOnlyList<ItemData> Slots => _slots;

    public bool TryAddIngredient(ItemData item)
    {
        if (_slots.Count >= MaxSlots) return false;
        _slots.Add(item);
        return true;
    }

    public void RemoveIngredient(ItemData item) => _slots.Remove(item);
    public void Clear() => _slots.Clear();
}