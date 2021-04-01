using System;
using System.Collections.Generic;

namespace RecipeBook.Core.Domain.Recipes
{
    public class Recipe : BaseEntity
    {
        public string                    Name          { get; set; } = string.Empty;
        public ICollection<UsedOccasion> UsedOccasions { get; set; } = new List<UsedOccasion>();
        public ICollection<Step>         Steps         { get; set; } = new List<Step>();
        public ICollection<Ingredient>   Ingredients   { get; set; } = new List<Ingredient>();
    }
}