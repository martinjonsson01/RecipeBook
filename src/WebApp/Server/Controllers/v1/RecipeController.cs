using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
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
    public class RecipeController : BaseApiController<RecipeController>
    {
        private readonly IRecipeRepository _repo;

        public RecipeController(
            ILogger<RecipeController> logger,
            IRecipeRepository         repo) 
            : base(logger)
        {
            _repo = repo;
        }

        /// <summary>
        /// Gets a Recipe by ID.
        /// </summary>
        /// <returns>A Recipe with matching ID</returns>
        /// <response code="200">Returns the matching Recipe</response>
        /// <response code="404">If no recipe with matching ID is found</response>  
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRecipe(int id)
        {
            Recipe? recipe = await _repo.FetchRecipe(id);
            if (recipe is null) return NotFound();
            return Ok(recipe);
        }

        /// <summary>
        /// Gets all recipes
        /// </summary>
        /// <returns>All recipes</returns>
        /// <response code="200">Returns the recipes</response>
        /// <response code="204">If there are no recipes</response>  
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetAllRecipes()
        {
            IEnumerable<Recipe> recipes = await _repo.FetchAllRecipes();
            if (!recipes.Any()) return NoContent();
            return Ok(recipes);
        }
    }
}