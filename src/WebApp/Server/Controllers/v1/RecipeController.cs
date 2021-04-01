using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace RecipeBook.Presentation.WebApp.Server.Controllers.v1
{
    /// <summary>
    /// Endpoints for CRUD of recipes.
    /// </summary>
    [ApiVersion("1.0")]
    public class RecipeController : BaseApiController<RecipeController>
    {
        public RecipeController(ILogger<RecipeController> logger) : base(logger)
        {
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
            await Task.CompletedTask;
            return Ok();
        }
    }
}