using Microsoft.Extensions.DependencyInjection;

using RecipeBook.Core.Application;

namespace RecipeBook.Infrastructure.Persistence
{
    public static class DependencyInjection
    {
        public static void AddPersistence(this IServiceCollection services)
        {
            services.AddDbContext<IRecipeRepository, RecipeBookDbContext>();
        }
    }
}