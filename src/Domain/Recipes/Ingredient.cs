using RecipeBook.Core.Domain.Units;

namespace RecipeBook.Core.Domain.Recipes
{
    public class Ingredient : BaseEntity
    {
        public string Name   { get; set; } = string.Empty;
        public Unit   Amount { get; set; } = new Mass(0);
    }
}