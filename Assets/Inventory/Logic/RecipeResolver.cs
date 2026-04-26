using System.Collections.Generic;

public class RecipeResolver
{
    private List<RecipeData> _recipes;
    private ItemData _defaultResult;
    private int _minimumIngredientsForDefault;

    public RecipeResolver(List<RecipeData> recipes, ItemData defaultResult = null, int minimumIngredientsForDefault = 2)
    {
        _recipes = recipes;
        _defaultResult = defaultResult;
        _minimumIngredientsForDefault = minimumIngredientsForDefault;
    }

    public ItemData TryResolve(IReadOnlyList<ItemData> ingredients)
    {
        foreach (var recipe in _recipes)
        {
            if (IsMatch(recipe.ingredients, ingredients))
                return recipe.result;
        }

        if (_defaultResult != null && ingredients.Count >= _minimumIngredientsForDefault)
            return _defaultResult;

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