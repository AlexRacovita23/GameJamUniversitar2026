using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "Crafting/Recipe")]
public class RecipeData : ScriptableObject
{
    public List<ItemData> ingredients;
    public ItemData result;
}