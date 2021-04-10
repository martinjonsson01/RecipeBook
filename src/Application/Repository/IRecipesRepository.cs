using System.Collections.Generic;
using System.Threading.Tasks;

using RecipeBook.Core.Domain.Recipes;

namespace RecipeBook.Core.Application.Repository
{
    public interface IRecipesRepository
    {
        public Task<Recipe?>             FetchAsync(string name);
        public Task<IEnumerable<Recipe>> FetchAllAsync();
        public Task<Recipe>              StoreAsync(Recipe  recipe);
        public Task                      DeleteAsync(string name);
        public Task<Recipe>              UpdateAsync(Recipe recipe);
    }
}