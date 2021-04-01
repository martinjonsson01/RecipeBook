using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace RecipeBook.Presentation.WebApp.Server.Pages
{
    /// <summary>
    /// Model for the error-page.
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        /// <summary>
        /// ID of the request that caused the error.
        /// </summary>
        public string RequestId { get; set; } = string.Empty;

        /// <summary>
        /// Whether or not to show the ID.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        private readonly ILogger<ErrorModel> _logger;

        /// <summary>
        /// Instantiates a new <see cref="ErrorModel"/>.
        /// </summary>
        /// <param name="logger">Dependency injected.</param>
        public ErrorModel(ILogger<ErrorModel> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Called when a GET-request is sent for the Error-page.
        /// </summary>
        public void OnGet()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            _logger.LogError("Error page shown for request id {Id}", RequestId);
        }
    }
}
