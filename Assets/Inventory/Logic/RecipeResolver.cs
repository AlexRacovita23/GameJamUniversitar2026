using System.Collections.Generic;

public class RecipeResolver
{
    private List<RecipeData> _recipes;

    public RecipeResolver(List<RecipeData> recipes)
    {
        _recipes = recipes;
    }

    public RecipeData TryResolve(IReadOnlyList<ItemData> ingredients)
    {
        foreach (var recipe in _recipes)
        {
            if (IsMatch(recipe.ingredients, ingredients))
                return recipe;
        }
        return null;
    }

    private bool IsMatch(List<ItemData> required, IReadOnlyList<ItemData> provided)
    {
        if (required.Count != provided.Count) return false;
        var copy = new List<ItemData>(provided);
        foreach (var item in required)
        {
            if (!copy.Remove(item)) return false;
        }
        return true;
    }
}