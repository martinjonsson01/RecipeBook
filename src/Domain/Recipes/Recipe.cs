using System;
using System.Collections.Generic;
using System.Linq;

namespace RecipeBook.Core.Domain.Recipes
{
    public class Recipe : BaseEntity, IShallowCloneable<Recipe>, IEquatable<Recipe>
    {
        public string              Name          { get; set; }          = string.Empty;
        public int?                Rating        { get; set; }          = null;
        public string?             ImagePath     { get; set; }          = null;
        public IList<UsedOccasion> UsedOccasions { get; private init; } = new List<UsedOccasion>();
        public IList<Step>         Steps         { get; init; }         = new List<Step>();
        public IList<Ingredient>   Ingredients   { get; init; }         = new List<Ingredient>();

        public string ToUrlSafeName()
        {
            return ToUrlSafeName(Name);
        }

        public static string ToUrlSafeName(string name)
        {
            return Uri.EscapeDataString(name).Replace("%20", "-");
        }

        public static string FromUrlSafeNameToOrdinaryName(string urlSafeName)
        {
            string escapedString = urlSafeName.Replace("-", "%20");
            return Uri.UnescapeDataString(escapedString);
        }

        public Recipe ShallowClone()
        {
            return new()
            {
                Id = Id,
                Name = Name,
                Rating = Rating,
                ImagePath = ImagePath,
                Steps = Steps,
                Ingredients = Ingredients,
                UsedOccasions = UsedOccasions
            };
        }

        public bool Equals(Recipe? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name && 
                   Rating == other.Rating && 
                   ImagePath == other.ImagePath && 
                   ItemEquals(UsedOccasions, other.UsedOccasions) &&
                   ItemEquals(Steps,         other.Steps) &&
                   ItemEquals(Ingredients, other.Ingredients);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Recipe) obj);
        }

        private bool ItemEquals<T>(IList<T> otherItems, IList<T> items)
            where T : notnull
        {
            if (otherItems.Count != items.Count) return false;
            return !otherItems.Where((item, index) => !item.Equals(items[index]))
                              .Any();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Name.GetHashCode();
                hashCode = (hashCode * 397) ^ Rating.GetHashCode();
                hashCode = (hashCode * 397) ^ (ImagePath != null ? ImagePath.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ UsedOccasions.GetHashCode();
                hashCode = (hashCode * 397) ^ Steps.GetHashCode();
                hashCode = (hashCode * 397) ^ Ingredients.GetHashCode();
                return hashCode;
            }
        }
    }
}