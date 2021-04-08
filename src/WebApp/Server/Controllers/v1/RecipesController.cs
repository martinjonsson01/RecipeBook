using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using RecipeBook.Core.Application;
using RecipeBook.Core.Domain.Recipes;

namespace RecipeBook.Presentation.WebApp.Server.Controllers.v1
{
    /// <summary>
    /// Endpoints for CRUD of recipes.
    /// </summary>
    [ApiVersion("1.0")]
    public class RecipesController : BaseApiController<RecipesController>
    {
        private readonly IRecipeRepository _repo;

        public RecipesController(
            ILogger<RecipesController> logger,
            IRecipeRepository         repo)
            : base(logger)
        {
            _repo = repo;
        }

        /// <summary>
        /// Gets a recipe by name.
        /// </summary>
        /// <param name="name">The name of the recipe</param>
        /// <returns>A Recipe with matching name</returns>
        /// <response code="200">Returns the matching recipe</response>
        /// <response code="404">If no recipe with matching name is found</response>
        [HttpGet("{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Recipe>> GetRecipe(string name)
        {
            Recipe? recipe = await _repo.FetchAsync(name);
            if (recipe is null) return NotFound();
            return Ok(recipe);
        }

        /// <summary>
        /// Gets all recipes.
        /// </summary>
        /// <returns>All recipes</returns>
        /// <response code="200">Returns the recipes</response>
        /// <response code="204">If there are no recipes</response>  
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetAllRecipes()
        {
            IEnumerable<Recipe> recipes = await _repo.FetchAllAsync();
            if (!recipes.Any()) return NoContent();
            return Ok(recipes);
        }

        /// <summary>
        /// Creates a new recipe in the database.
        /// </summary>
        /// <remarks>
        /// Example body:
        ///
        ///     POST /
        ///     {
        ///         "name": "Test Recipe",
        ///         "rating": 10,
        ///         "usedOccasions": [],
        ///         "steps": [],
        ///         "ingredients": []
        ///     }
        /// </remarks>
        /// <returns>The location of the created recipe</returns>
        /// <response code="201">Returns the created recipe</response>
        /// <response code="409">If a recipe with provided name already exists</response>  
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<Recipe>> PostRecipe(Recipe recipe)
        {
            if (await _repo.FetchAsync(recipe.Name) is not null)
                return Conflict();

            await _repo.StoreAsync(recipe);

            return CreatedAtAction(nameof(GetRecipe), new { name = recipe.Name }, recipe);
        }

        /// <summary>
        /// Deletes a recipe by name.
        /// </summary>
        /// <param name="name">The name of the recipe</param>
        /// <response code="200">The recipe was deleted</response>
        /// <response code="404">The recipe does not exist</response>
        [HttpDelete("{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRecipe(string name)
        {
            if (await _repo.FetchAsync(name) is null)
                return NotFound();

            await _repo.DeleteAsync(name);
            return Ok();
        }

        /// <summary>
        /// Updates an existing recipe in the database.
        /// </summary>
        /// <remarks>
        /// Example body:
        ///
        ///     PATCH /
        ///     [
        ///       {
        ///         "op": "replace",
        ///         "path": "/rating",
        ///         "value": 5
        ///       },
        ///       {
        ///         "op": "remove",
        ///         "path": "/rating"
        ///       }
        ///     ]
        /// </remarks>
        /// <response code="204">Recipe was successfully updated</response>
        /// <response code="400">Patch caused invalid state</response>
        /// <response code="404">Recipe does not exist</response>
        [HttpPatch("{name}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] 
        public async Task<IActionResult> PatchRecipe(string name, [FromBody] JsonPatchDocument<Recipe> patch)
        {
            Recipe? recipe = await _repo.FetchAsync(name);
            if (recipe is null) return NotFound();

            patch.ApplyTo(recipe, ModelState);

            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            await _repo.UpdateAsync(recipe);
            return NoContent();
        }
    }
}