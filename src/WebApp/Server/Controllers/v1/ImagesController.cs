using System.IO;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using RecipeBook.Core.Application.Exceptions;
using RecipeBook.Core.Application.FileStorage;

namespace RecipeBook.Presentation.WebApp.Server.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Recipes/{recipeName}/[Controller]")]
    public class ImagesController : BaseApiController<ImagesController>
    {
        public ImagesController(
            ILogger<ImagesController>               logger,
            IFileStorer fileStorer) : base(logger)
        {
            _fileStorer = fileStorer;
        }

        private readonly IFileStorer _fileStorer;

        /// <summary>
        /// Gets an image by name.
        /// </summary>
        /// <param name="recipeName">The name of the recipe of this image</param>
        /// <param name="imageName">The name of the image</param>
        /// <returns>An image with matching name</returns>
        /// <response code="200">Returns the matching image</response>
        /// <response code="404">If no image with matching name is found</response>
        [HttpGet("{imageName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ApiExplorerSettings(IgnoreApi = false)] 
        public IActionResult GetImage(string recipeName, string imageName)
        {
            if (!_fileStorer.Exists(imageName)) return NotFound();
            FileStream fileStream = _fileStorer.LoadFile(imageName);
            return File(fileStream, "image/jpeg");
        }

        /// <summary>
        /// Uploads an image.
        /// </summary>
        /// <returns>The location of the stored image</returns>
        /// <param name="recipeName">The name of the recipe of this image</param>
        /// <param name="image">The image to upload</param>
        /// <response code="201">Returns the location of the stored image</response>
        /// <response code="406">If image length is zero</response>  
        /// <response code="413">If image is too large</response>  
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(StatusCodes.Status413RequestEntityTooLarge)]
        [ApiExplorerSettings(IgnoreApi = false)] 
        public IActionResult PostImage(IFormFile image, string recipeName)
        {
            try
            {
                Stream  readStream    = image.OpenReadStream();
                string fileName = _fileStorer.SaveFile(readStream);
                return CreatedAtAction(
                    $"{nameof(GetImage)}",
                    new { imageName = fileName, recipeName }, image);
            }
            catch (FileLengthZeroException)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable);
            }
            catch (FileTooLargeException)
            {
                return StatusCode(StatusCodes.Status413RequestEntityTooLarge);
            }
        }
    }
}