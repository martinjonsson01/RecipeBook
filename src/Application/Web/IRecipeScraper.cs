using System.Threading.Tasks;

using RecipeBook.Core.Domain.Recipes;

namespace RecipeBook.Core.Application.Web
{
    public interface IRecipeScraper
    {
        Task<Recipe?> Scrape(string fromUrl);
    }
}