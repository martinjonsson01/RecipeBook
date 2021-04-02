using System.Collections.Generic;
using System.Threading.Tasks;

using RecipeBook.Core.Domain.Recipes;

namespace RecipeBook.Core.Application
{
    public interface IRecipeRepository
    {
        public Task<Recipe?>             FetchRecipe(int id);
        public Task<IEnumerable<Recipe>> FetchAllRecipes();
    }
}