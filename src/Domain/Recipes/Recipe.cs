using System;
using System.Collections.Generic;
using System.Linq;

namespace RecipeBook.Core.Domain.Recipes
{
    public class Recipe : BaseEntity
    {
        public string              Name          { get; set; }          = string.Empty;
        public int?                Rating        { get; set; }          = null;
        public string?             ImagePath     { get; set; }          = null;
        public IList<UsedOccasion> UsedOccasions { get; private init; } = new List<UsedOccasion>();
        public IList<Step>         Steps         { get; private init; } = new List<Step>();
        public IList<Ingredient>   Ingredients   { get; private init; } = new List<Ingredient>();

        public string ToUrlSafeName()
        {
            return Uri.EscapeUriString(Name).Replace("%20", "-");
        }

        public static string FromUrlSafeNameToOrdinaryName(string urlSafeName)
        {
            string escapedString = urlSafeName.Replace("-", "%20");
            return Uri.UnescapeDataString(escapedString);
        }

        public override bool Equals(object? obj)
        {
            if (obj is Recipe other) return Equals(other);
            return false;
        }

        public Recipe Clone()
        {
            return new()
            {
                Id = Id,
                Name = Name,
                Rating = Rating,
                Steps = Steps,
                Ingredients = Ingredients,
                UsedOccasions = UsedOccasions
            };
        }

        public bool Equals(Recipe? other)
        {
            if (other is null) return false;
            return other.Id == Id &&
                   other.Name.Equals(Name) &&
                   other.Rating == Rating &&
                   ItemEquals(other.Steps,         Steps) &&
                   ItemEquals(other.Ingredients,   Ingredients) &&
                   ItemEquals(other.UsedOccasions, UsedOccasions);
        }

        private bool ItemEquals<T>(IList<T> otherItems, IList<T> items)
            where T : notnull
        {
            if (otherItems.Count != items.Count) return false;
            return !otherItems.Where((item, index) => !item.Equals(items[index]))
                              .Any();
        }
    }
}