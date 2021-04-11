using Microsoft.Extensions.DependencyInjection;

using RecipeBook.Core.Application.Repository;
using RecipeBook.Core.Domain.Recipes;
using RecipeBook.Infrastructure.Persistence.Repositories;

namespace RecipeBook.Infrastructure.Persistence
{
    public static class DependencyInjection
    {
        public static void AddPersistence(this IServiceCollection services)
        {
            services.AddTransient<IResourcesRepository<Recipe, string>, RecipesRepository>();
            services.AddTransient<IResourcesRepository<UsedOccasion, int?>, UsedOccasionsRepository>();
        }
    }
}