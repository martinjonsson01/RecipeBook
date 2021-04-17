using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using RecipeBook.Core.Application.Repositories;

namespace RecipeBook.Presentation.WebApp.Server.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Recipes/{recipeName}/[Controller]")]
    public abstract class ResourceController<TController, TResource, TResourceKey>
        : BaseApiController<TController>
        where TResource : class
        where TController : class
    {
        public ResourceController(
            ILogger<TController>                          logger,
            IResourcesRepository<TResource, TResourceKey> repo)
            : base(logger)
        {
            _repo = repo;
        }

        protected readonly IResourcesRepository<TResource, TResourceKey> _repo;

        /// <summary>
        /// Gets all resources.
        /// </summary>
        /// <param name="recipeName">The name of the recipe containing these resources</param>
        /// <returns>All resources</returns>
        /// <response code="200">Returns the resources</response>
        /// <response code="204">If there are no resources</response>
        [ApiExplorerSettings(IgnoreApi = true)] 
        public virtual async Task<ActionResult<IEnumerable<TResource>>> GetAll(string recipeName)
        {
            IEnumerable<TResource> resources = await _repo.GetAllAsync(recipeName);
            if (!resources.Any()) return NoContent();
            return Ok(resources);
        }

        /// <summary>
        /// Gets a resource by key.
        /// </summary>
        /// <param name="recipeName">The name of the recipe containing this resource</param>
        /// <param name="id">The key of the resource</param>
        /// <returns>A resource with matching key</returns>
        /// <response code="200">Returns the matching resource</response>
        /// <response code="404">If no resource with matching key is found</response>
        [ApiExplorerSettings(IgnoreApi = true)] 
        public virtual async Task<ActionResult<TResource>> Get(string recipeName, TResourceKey id)
        {
            TResource? entity = await _repo.GetAsync(recipeName, id);
            if (entity is null) return NotFound();
            return Ok(entity);
        }

        /// <summary>
        /// Creates a new or updates an existing resource.
        /// </summary>
        /// <param name="recipeName">The name of the recipe containing this resource</param>
        /// <param name="entity">The resource to create or update</param>
        /// <returns>A created or updated resource</returns>
        /// <response code="201">If a new resource was created</response>
        /// <response code="200">If an existing resource was updated</response>
        /// <response code="400">If a provided entity is wrong</response>
        [ApiExplorerSettings(IgnoreApi = true)] 
        public virtual async Task<ActionResult<TResource>> CreateOrUpdate(string recipeName, TResource entity)
        {
            TResourceKey key = GetKey(entity);
            if (await _repo.ExistsAsync(recipeName, key))
            {
                TResource? updatedEntity = await _repo.CreateOrUpdateAsync(recipeName, entity);
                if (updatedEntity is null) return BadRequest();
                return Ok(updatedEntity);
            }
            TResource? createdEntity = await _repo.CreateOrUpdateAsync(recipeName, entity);
            if (createdEntity is null) return BadRequest();
            return CreatedAtAction(
                $"{nameof(Get)}",
                new { id = key, recipeName },
                createdEntity);
        }

        /// <summary>
        /// Deletes a resource by key.
        /// </summary>
        /// <param name="recipeName">The name of the recipe containing this resource</param>
        /// <param name="id">The key of the resource</param>
        /// <response code="200">The resource was deleted</response>
        /// <response code="404">The resource does not exist</response>
        /// <response code="500">The server failed to delete the resource</response>
        [ApiExplorerSettings(IgnoreApi = true)] 
        public virtual async Task<ActionResult> Delete(string recipeName, TResourceKey id)
        {
            if (!await _repo.ExistsAsync(recipeName, id))
                return NotFound();

            await _repo.DeleteAsync(recipeName, id);
            return Ok();
        }

        /// <summary>
        /// Gets the key of a given entity.
        /// </summary>
        protected abstract TResourceKey GetKey(TResource entity);
    }
}