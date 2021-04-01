using System;

namespace RecipeBook.Core.Domain.Recipes
{
    public class TimeStep : Step
    {
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;
    }
}