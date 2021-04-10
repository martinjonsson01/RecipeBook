using System.Collections.Generic;
using System.Threading.Tasks;

namespace RecipeBook.Core.Application.Repository
{
    public interface IResourcesRepository<TResource, in TKey>
    {
        /// <summary>
        /// Gets all resources under provided recipe name.
        /// </summary>
        /// <param name="recipeName">The name of the recipe that the resources belong to</param>
        /// <returns>The resources belonging to the recipe</returns>
        public Task<IEnumerable<TResource>> GetAllAsync(string recipeName);

        /// <summary>
        /// Gets a resource under provided recipe name.
        /// </summary>
        /// <param name="recipeName">The name of the recipe that the resource belongs to</param>
        /// <param name="key">The identifying key of the resource to get</param>
        /// <returns>The corresponding resource, if it exists, otherwise null</returns>
        public Task<TResource?> GetAsync(string recipeName, TKey key);

        /// <summary>
        /// Checks whether or not a resource exists under a recipe name.
        /// </summary>
        /// <param name="recipeName">The name of the recipe to check under</param>
        /// <param name="key">The identifying key of the resource to check</param>
        /// <returns>Whether or not it exists</returns>
        public Task<bool> ExistsAsync(string recipeName, TKey key);

        /// <summary>
        /// Creates a new resource under provided recipe name if one does not already exist,
        /// or updates the resource if it already exists.
        /// </summary>
        /// <param name="recipeName">The name of the recipe that the resource belongs to</param>
        /// <param name="entity">The resource to create or update</param>
        /// <returns>The created or updated resource, with correct key identifier.
        /// Might be null if some conflict occured</returns>
        public Task<TResource?> CreateOrUpdateAsync(string recipeName, TResource entity);

        /// <summary>
        /// Removes a resource from under a recipe name.
        /// </summary>
        /// <param name="recipeName">The name of the recipe to remove a resource from</param>
        /// <param name="key">The identifying key of the resource to be removed</param>
        public Task DeleteAsync(string recipeName, TKey key);
    }
}