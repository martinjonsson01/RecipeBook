using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using RecipeBook.Core.Application.Repositories;
using RecipeBook.Core.Domain.Recipes;

namespace RecipeBook.Presentation.WebApp.Server.Controllers.v1
{
    /**
     * This subclass of ResourceController is merely a facade with documentation applied.
     * The actual implementation is in the base class.
     */
    [ApiVersion("1.0")]
    public class StepsController : ResourceController<StepsController, Step, int?>
    {
        public StepsController(
            ILogger<StepsController>         logger,
            IResourcesRepository<Step, int?> repo)
            : base(logger, repo)
        {
        }

        protected override int? GetKey(Step entity) => entity.Id;
        
        /// <summary>
        /// Gets all steps.
        /// </summary>
        /// <param name="recipeName">The name of the recipe containing the steps</param>
        /// <returns>All steps</returns>
        /// <response code="200">Returns the steps</response>
        /// <response code="204">If there are no steps</response>  
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ApiExplorerSettings(IgnoreApi = false)] 
        public override Task<ActionResult<IEnumerable<Step>>> GetAll(string recipeName)
        {
            return base.GetAll(recipeName);
        }

        /// <summary>
        /// Gets a step by Id.
        /// </summary>
        /// <param name="recipeName">The name of the recipe containing this step</param>
        /// <param name="id">The Id of the step</param>
        /// <returns>A step with matching Id</returns>
        /// <response code="200">Returns the matching step</response>
        /// <response code="404">If no step with matching Id is found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ApiExplorerSettings(IgnoreApi = false)] 
        public override Task<ActionResult<Step>> Get(string recipeName, int? id)
        {
            return base.Get(recipeName, id);
        }

        /// <summary>
        /// Creates a new or updates an existing step.
        /// </summary>
        /// <param name="recipeName">The name of the recipe containing this resource</param>
        /// <param name="step">The step to create or update</param>
        /// <returns>A created or updated step</returns>
        /// <response code="201">If a new step was created</response>
        /// <response code="200">If an existing step was updated</response>
        /// <response code="400">If a provided step is wrong</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ApiExplorerSettings(IgnoreApi = false)] 
        public override Task<ActionResult<Step>> CreateOrUpdate(string recipeName, Step step)
        {
            return base.CreateOrUpdate(recipeName, step);
        }

        /// <summary>
        /// Deletes a step by Id.
        /// </summary>
        /// <param name="recipeName">The name of the recipe containing this step</param>
        /// <param name="id">The key of the step</param>
        /// <response code="200">The step was deleted</response>
        /// <response code="404">The step does not exist</response>
        /// <response code="500">The server failed to delete the step</response>
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