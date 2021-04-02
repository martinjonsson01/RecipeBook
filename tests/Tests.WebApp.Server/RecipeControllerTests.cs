using System;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

using RecipeBook.Core.Application;
using RecipeBook.Core.Domain.Recipes;
using RecipeBook.Presentation.WebApp.Server.Controllers.v1;

using Xunit;

namespace Tests.WebApp.Server
{
    public class RecipeControllerTests
    {
        private readonly RecipeController        _controller;
        private readonly Mock<IRecipeRepository> _mockRepo;

        public RecipeControllerTests()
        {
            _mockRepo = new Mock<IRecipeRepository>(); 
            var mockLogger = new Mock<ILogger<RecipeController>>();
            _controller = new RecipeController(mockLogger.Object, _mockRepo.Object);
        }

        [Fact]
        private async Task GetRecipe_ReturnsOkWithObject_WithExistingRecipe()
        {
            // Arrange
            var mockRecipe = new Mock<Recipe>();
            _mockRepo.Setup(repo => repo.FetchRecipe(0)).ReturnsAsync(mockRecipe.Object);

            // Act
            IActionResult result = await _controller.GetRecipe(0);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }
        
        [Fact]
        private async Task GetRecipe_ReturnsNotFound_WithNoRecipe()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.FetchRecipe(0)).ReturnsAsync((Recipe) null);
            
            // Act
            IActionResult result = await _controller.GetRecipe(0);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        private async Task GetAllRecipes_ReturnsOkWithObject_WithExistingRecipes()
        {
            // Arrange
            var      mockRecipe  = new Mock<Recipe>();
            Recipe[] mockRecipes = { mockRecipe.Object };
            _mockRepo.Setup(repo => repo.FetchAllRecipes()).ReturnsAsync(mockRecipes);

            // Act
            IActionResult result = await _controller.GetAllRecipes();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        private async Task GetAllRecipes_ReturnsNoContent_WithNoRecipes()
        {
            // Arrange
            Recipe[] mockRecipes = Array.Empty<Recipe>();
            _mockRepo.Setup(repo => repo.FetchAllRecipes()).ReturnsAsync(mockRecipes);

            // Act
            IActionResult result = await _controller.GetAllRecipes();

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }
    }
}