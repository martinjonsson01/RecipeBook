using System;

namespace RecipeBook.Core.Domain.Recipes
{
    public class UsedOccasion
    {
        public DateTime When     { get; set; } = DateTime.MinValue;
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;
        public string   Comment  { get; set; } = string.Empty;
    }
}