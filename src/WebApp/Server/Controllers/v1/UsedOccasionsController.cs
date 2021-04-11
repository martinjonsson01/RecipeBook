using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using RecipeBook.Core.Application.Repository;
using RecipeBook.Core.Domain.Recipes;

namespace RecipeBook.Presentation.WebApp.Server.Controllers.v1
{
    [ApiVersion("1.0")]
    public class UsedOccasionsController : ResourceController<UsedOccasionsController, UsedOccasion, int?>
    {
        public UsedOccasionsController(
            ILogger<UsedOccasionsController>       logger,
            IResourcesRepository<UsedOccasion, int?> repo)
            : base(logger, repo)
        {
        }

        protected override int? GetKey(UsedOccasion entity) => entity.Id;
        
        /// <summary>
        /// Gets all used occasions.
        /// </summary>
        /// <param name="recipeName">The name of the recipe containing the used occasions</param>
        /// <returns>All used occasions</returns>
        /// <response code="200">Returns the used occasions</response>
        /// <response code="204">If there are no used occasions</response>  
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ApiExplorerSettings(IgnoreApi = false)] 
        public override Task<ActionResult<IEnumerable<UsedOccasion>>> GetAll(string recipeName)
        {
            return base.GetAll(recipeName);
        }

        /// <summary>
        /// Gets a used occasion by Id.
        /// </summary>
        /// <param name="recipeName">The name of the recipe containing this used occasion</param>
        /// <param name="id">The Id of the used occasion</param>
        /// <returns>A used occasion with matching Id</returns>
        /// <response code="200">Returns the matching used occasion</response>
        /// <response code="404">If no used occasion with matching Id is found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ApiExplorerSettings(IgnoreApi = false)] 
        public override Task<ActionResult<UsedOccasion>> Get(string recipeName, int? id)
        {
            return base.Get(recipeName, id);
        }

        /// <summary>
        /// Creates a new or updates an existing used occasion.
        /// </summary>
        /// <param name="recipeName">The name of the recipe containing this resource</param>
        /// <param name="usedOccasion">The used occasion to create or update</param>
        /// <returns>A created or updated used occasion</returns>
        /// <response code="201">If a new used occasion was created</response>
        /// <response code="200">If an existing used occasion was updated</response>
        /// <response code="400">If a provided used occasion is wrong</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ApiExplorerSettings(IgnoreApi = false)] 
        public override Task<ActionResult<UsedOccasion>> CreateOrUpdate(string recipeName, UsedOccasion usedOccasion)
        {
            return base.CreateOrUpdate(recipeName, usedOccasion);
        }

        /// <summary>
        /// Deletes a used occasion by Id.
        /// </summary>
        /// <param name="recipeName">The name of the recipe containing this used occasion</param>
        /// <param name="id">The key of the used occasion</param>
        /// <response code="200">The used occasion was deleted</response>
        /// <response code="404">The used occasion does not exist</response>
        /// <response code="500">The server failed to delete the used occasion</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ApiExplorerSettings(IgnoreApi = false)] 
        public override Task<ActionResult> Delete(string recipeName, int? id)
        {
            return base.Delete(recipeName, id);
        }
    }
}