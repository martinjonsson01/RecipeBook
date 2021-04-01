using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace RecipeBook.Presentation.WebApp.Server.Controllers.v1
{
    /// <summary>
    /// Endpoint for CRUD of recipes.
    /// </summary>
    [ApiVersion("1.0")]
    public class RecipeController : BaseApiController<RecipeController>
    {
        /// <summary>
        /// Injects logger.
        /// </summary>
        public RecipeController(ILogger<RecipeController> logger) : base(logger)
        {
        }
    }
}