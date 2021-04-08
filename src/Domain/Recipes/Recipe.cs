using System;
using System.Collections.Generic;

namespace RecipeBook.Core.Domain.Recipes
{
    public class Recipe : BaseEntity
    {
        public string              Name          { get; set; } = string.Empty;
        public int?                Rating        { get; set; } = null;
        public string?             ImagePath     { get; set; } = null;
        public IList<UsedOccasion> UsedOccasions { get; set; } = new List<UsedOccasion>();
        public IList<Step>         Steps         { get; set; } = new List<Step>();
        public IList<Ingredient>   Ingredients   { get; set; } = new List<Ingredient>();
        
        public string ToUrlSafeName()
        {
            return Uri.EscapeUriString(Name).Replace("%20", "-");
        }

        public static string FromUrlSafeNameToOrdinaryName(string urlSafeName)
        {
            string escapedString = urlSafeName.Replace("-", "%20");
            return Uri.UnescapeDataString(escapedString);
        }
    }
}