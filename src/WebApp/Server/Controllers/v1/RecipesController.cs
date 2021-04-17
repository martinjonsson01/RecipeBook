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
     * Note: Since this controller is top-level but still has to implement
     * ResourceController (which assumes that the controller is a subset of /Recipes/)
     * all parameters requiring a recipeName will be replaced with string.empty.
     */
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[Controller]")]
    public class RecipesController : ResourceController<RecipesController, Recipe, string>
    {
        public RecipesController(
            ILogger<RecipesController>       logger,
            IResourcesRepository<Recipe, string> repo)
            : base(logger, repo)
        {
        }

        protected override string GetKey(Recipe entity) => entity.Name;
        
        /// <summary>
        /// Gets all recipes.
        /// </summary>
        /// <returns>All recipes</returns>
        /// <param name="unused">Has no effect on response</param>
        /// <response code="200">Returns the recipes</response>
        /// <response code="204">If there are no recipes</response>  
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ApiExplorerSettings(IgnoreApi = false)]
        public override Task<ActionResult<IEnumerable<Recipe>>> GetAll(string? unused)
        {
            return base.GetAll(unused ?? string.Empty);
        }

        /// <summary>
        /// Gets a image by name.
        /// </summary>
        /// <param name="unused">Has no effect on response</param>
        /// <param name="id">The name of the image</param>
        /// <returns>A image with matching name</returns>
        /// <response code="200">Returns the matching image</response>
        /// <response code="404">If no image with matching name is found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ApiExplorerSettings(IgnoreApi = false)]
        public override Task<ActionResult<Recipe>> Get(string? unused, string id)
        {
            return base.Get(unused ?? string.Empty, Recipe.FromUrlSafeNameToOrdinaryName(id));
        }

        /// <summary>
        /// Creates a new or updates an existing image.
        /// </summary>
        /// <param name="unused">Has no effect on response</param>
        /// <param name="image">The image to create or update</param>
        /// <returns>A created or updated image</returns>
        /// <response code="201">If a new image was created</response>
        /// <response code="200">If an existing image was updated</response>
        /// <response code="400">If image name is already taken</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ApiExplorerSettings(IgnoreApi = false)]
        public override Task<ActionResult<Recipe>> CreateOrUpdate(string? unused, Recipe image)
        {
            return base.CreateOrUpdate(unused ?? string.Empty, image);
        }

        /// <summary>
        /// Deletes a image by name.
        /// </summary>
        /// <param name="unused">Has no effect on response</param>
        /// <param name="id">The name of the image</param>
        /// <response code="200">The image was deleted</response>
        /// <response code="404">The image does not exist</response>
        /// <response code="500">The server failed to delete the image</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ApiExplorerSettings(IgnoreApi = false)]
        public override Task<ActionResult> Delete(string? unused, string id)
        {
            return base.Delete(unused ?? string.Empty, id);
        }
    }
}