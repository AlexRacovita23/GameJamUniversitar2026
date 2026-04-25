using System.Collections.Generic;

public class CraftingTable
{
    public int MaxSlots = 3;

    private List<ItemData> _slots = new();
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