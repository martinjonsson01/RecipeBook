using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

using RecipeBook.Core.Application;
using RecipeBook.Core.Domain.Recipes;
using RecipeBook.Presentation.WebApp.Server.Controllers.v1;

using Xunit;

namespace Tests.WebApp.Server
{
    public class RecipesControllerTests
    {
        private readonly RecipesController        _controller;
        private readonly Mock<IRecipeRepository> _mockRepo;
        private readonly Recipe                  _mockRecipe;

        public RecipesControllerTests()
        {
            _mockRecipe = new Recipe
            {
                Name = "Test Recipe"
            };
            _mockRepo = new Mock<IRecipeRepository>();
            var mockLogger = new Mock<ILogger<RecipesController>>();
            _controller = new RecipesController(mockLogger.Object, _mockRepo.Object);
        }

        [Fact]
        private async Task GetRecipe_ReturnsOkWithObject_WithExistingRecipe()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.FetchAsync(_mockRecipe.Name)).ReturnsAsync(_mockRecipe);

            // Act
            ActionResult<Recipe> result = await _controller.GetRecipe(_mockRecipe.Name);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        private async Task GetRecipe_ReturnsNotFound_WithNoRecipe()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.FetchAsync(It.IsAny<string>())).ReturnsAsync((Recipe) null);

            // Act
            ActionResult<Recipe> result = await _controller.GetRecipe(_mockRecipe.Name);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        private async Task GetAllRecipes_ReturnsOkWithObject_WithExistingRecipes()
        {
            // Arrange
            Recipe[] mockRecipes = { _mockRecipe };
            _mockRepo.Setup(repo => repo.FetchAllAsync()).ReturnsAsync(mockRecipes);

            // Act
            ActionResult<IEnumerable<Recipe>> result = await _controller.GetAllRecipes();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        private async Task GetAllRecipes_ReturnsNoContent_WithNoRecipes()
        {
            // Arrange
            Recipe[] mockRecipes = Array.Empty<Recipe>();
            _mockRepo.Setup(repo => repo.FetchAllAsync()).ReturnsAsync(mockRecipes);

            // Act
            ActionResult<IEnumerable<Recipe>> result = await _controller.GetAllRecipes();

            // Assert
            result.Result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        private async Task PostRecipe_CallsRepoStoreMethod_WithNoRecipe()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.StoreAsync(It.IsAny<Recipe>()));

            // Act
            await _controller.PostRecipe(_mockRecipe);

            // Assert
            _mockRepo.Verify(repo => repo.StoreAsync(_mockRecipe), Times.Once);
        }

        [Fact]
        private async Task PostRecipe_DoesNotCallRepoStoreMethod_WithExistingRecipe()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.FetchAsync(_mockRecipe.Name)).ReturnsAsync(_mockRecipe);
            _mockRepo.Setup(repo => repo.StoreAsync(It.IsAny<Recipe>()));

            // Act
            await _controller.PostRecipe(_mockRecipe);

            // Assert
            _mockRepo.Verify(repo => repo.StoreAsync(_mockRecipe), Times.Never);
        }

        [Fact]
        private async Task PostRecipe_ReturnsConflict_WithExistingRecipe()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.FetchAsync(_mockRecipe.Name)).ReturnsAsync(_mockRecipe);

            // Act
            ActionResult<Recipe> result = await _controller.PostRecipe(_mockRecipe);

            // Assert
            result.Result.Should().BeOfType<ConflictResult>();
        }

        [Fact]
        private async Task DeleteRecipe_ReturnsOk_WithExistingRecipe()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.FetchAsync(_mockRecipe.Name)).ReturnsAsync(_mockRecipe);

            // Act
            IActionResult result = await _controller.DeleteRecipe(_mockRecipe.Name);

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        private async Task DeleteRecipe_ReturnsNotFound_WithNoRecipe()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.FetchAsync(_mockRecipe.Name)).ReturnsAsync((Recipe) null);

            // Act
            IActionResult result = await _controller.DeleteRecipe(_mockRecipe.Name);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        private async Task PatchRecipe_WithNewRating_ReturnsNoContent_WithExistingRecipe()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.FetchAsync(_mockRecipe.Name)).ReturnsAsync(_mockRecipe);
            var patch = new JsonPatchDocument<Recipe>();
            patch.Replace(recipe => recipe.Rating, 5);

            // Act
            IActionResult result = await _controller.PatchRecipe(_mockRecipe.Name, patch);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        private async Task PatchRecipe_WithNewRating_ReturnsNotFound_WithNoRecipe()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.FetchAsync(_mockRecipe.Name)).ReturnsAsync((Recipe) null);
            var patch = new JsonPatchDocument<Recipe>();
            patch.Replace(recipe => recipe.Rating, 5);

            // Act
            IActionResult result = await _controller.PatchRecipe(_mockRecipe.Name, patch);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        private async Task PatchRecipe_WithNewRating_ReturnsBadRequest_WithExistingRecipe()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.FetchAsync(_mockRecipe.Name)).ReturnsAsync(_mockRecipe);
            var patch = new JsonPatchDocument<Recipe>();
            patch.Operations.Add(new Operation<Recipe>("replace", "/rating", "", 5.1f));

            // Act
            IActionResult result = await _controller.PatchRecipe(_mockRecipe.Name, patch);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}