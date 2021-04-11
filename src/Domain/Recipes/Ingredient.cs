using System;

using RecipeBook.Core.Domain.Units;

namespace RecipeBook.Core.Domain.Recipes
{
    public class Ingredient : BaseEntity
    {
        public string Name   { get; set; } = string.Empty;
        public Unit   Amount { get; set; } = new Mass(0);

        public static Ingredient MapFromRow(dynamic row)
        {
            var ingredient = new Ingredient
            {
                Id = row.ingredientid,
                Name = row.ingredientname,
                Amount = row.massid is not null
                    ? new Mass { Id = row.massid, Value = row.value }
                    : new Volume { Id = row.volumeid, Value = row.value }
            };
            return ingredient;
        }
    }
}