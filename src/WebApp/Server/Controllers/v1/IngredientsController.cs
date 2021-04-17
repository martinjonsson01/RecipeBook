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
    public class IngredientsController : ResourceController<IngredientsController, Ingredient, int?>
    {
        public IngredientsController(
            ILogger<IngredientsController>         logger,
            IResourcesRepository<Ingredient, int?> repo)
            : base(logger, repo)
        {
        }

        protected override int? GetKey(Ingredient entity) => entity.Id;
        
        /// <summary>
        /// Gets all ingredients.
        /// </summary>
        /// <param name="recipeName">The name of the recipe containing the ingredients</param>
        /// <returns>All ingredients</returns>
        /// <response code="200">Returns the ingredients</response>
        /// <response code="204">If there are no ingredients</response>  
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ApiExplorerSettings(IgnoreApi = false)] 
        public override Task<ActionResult<IEnumerable<Ingredient>>> GetAll(string recipeName)
        {
            return base.GetAll(recipeName);
        }

        /// <summary>
        /// Gets an ingredient by Id.
        /// </summary>
        /// <param name="recipeName">The name of the recipe containing this ingredient</param>
        /// <param name="id">The Id of the ingredient</param>
        /// <returns>A ingredient with matching Id</returns>
        /// <response code="200">Returns the matching ingredient</response>
        /// <response code="404">If no ingredient with matching Id is found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ApiExplorerSettings(IgnoreApi = false)] 
        public override Task<ActionResult<Ingredient>> Get(string recipeName, int? id)
        {
            return base.Get(recipeName, id);
        }

        /// <summary>
        /// Creates a new or updates an existing ingredient.
        /// </summary>
        /// <param name="recipeName">The name of the recipe containing this resource</param>
        /// <param name="ingredient">The ingredient to create or update</param>
        /// <returns>A created or updated ingredient</returns>
        /// <response code="201">If a new ingredient was created</response>
        /// <response code="200">If an existing ingredient was updated</response>
        /// <response code="400">If a provided ingredient is wrong</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ApiExplorerSettings(IgnoreApi = false)] 
        public override Task<ActionResult<Ingredient>> CreateOrUpdate(string recipeName, Ingredient ingredient)
        {
            return base.CreateOrUpdate(recipeName, ingredient);
        }

        /// <summary>
        /// Deletes an ingredient by Id.
        /// </summary>
        /// <param name="recipeName">The name of the recipe containing this ingredient</param>
        /// <param name="id">The key of the ingredient</param>
        /// <response code="200">The ingredient was deleted</response>
        /// <response code="404">The ingredient does not exist</response>
        /// <response code="500">The server failed to delete the ingredient</response>
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