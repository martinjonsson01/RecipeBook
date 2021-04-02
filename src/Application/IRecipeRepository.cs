using System.Collections.Generic;
using System.Threading.Tasks;

using RecipeBook.Core.Domain.Recipes;

namespace RecipeBook.Core.Application
{
    public interface IRecipeRepository
    {
        public Task<Recipe?>             FetchAsync(string name);
        public Task<IEnumerable<Recipe>> FetchAllAsync();
        public Task                      StoreAsync(Recipe  recipe);
        public Task                      DeleteAsync(string name);
        public Task                      UpdateAsync(Recipe recipe);
    }
}