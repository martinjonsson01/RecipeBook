using System.Collections.Generic;

namespace RecipeBook.Core.Domain.Recipes
{
    public class Recipe
    {
        public string              Name          { get; set; } = string.Empty;
        public int?                Rating        { get; set; } = null;
        public IList<UsedOccasion> UsedOccasions { get; set; } = new List<UsedOccasion>();
        public IList<Step>         Steps         { get; set; } = new List<Step>();
        public IList<Ingredient>   Ingredients   { get; set; } = new List<Ingredient>();
    }
}