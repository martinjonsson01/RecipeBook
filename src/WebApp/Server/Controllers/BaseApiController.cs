using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace RecipeBook.Presentation.WebApp.Server.Controllers
{
    /// <summary>
    /// Contains all the common components of the api controllers.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public abstract class BaseApiController<T> : ControllerBase
    {
        /// <summary>
        /// Interface for logging errors and information.
        /// </summary>
        protected readonly ILogger Logger;
        
        /// <summary>
        /// Injects logger.
        /// </summary>
        protected BaseApiController(ILogger<T> logger)
        {
            Logger = logger;
        }
    }
}