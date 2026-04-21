using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Crafting/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
}