using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace RecipeBook.Presentation.WebApp.Server.Controllers
{
    /// <summary>
    /// Contains all the common components of the api controllers.
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public abstract class BaseApiController<T> : ControllerBase
    {
        protected readonly ILogger Logger;
        
        protected BaseApiController(ILogger<T> logger)
        {
            Logger = logger;
        }
    }
}