using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

using RecipeBook.Core.Application;
using RecipeBook.Presentation.WebApp.Server.Controllers.v1;

using Xunit;

namespace Tests.WebApp.Server
{
    public class RecipeControllerTests
    {
        private readonly Mock<IRecipeRepository>         _mockRepo;
        private readonly Mock<ILogger<RecipeController>> _mockLogger;
        private readonly RecipeController                _controller;

        public RecipeControllerTests()
        {
            _mockRepo = new Mock<IRecipeRepository>();
            _mockLogger = new Mock<ILogger<RecipeController>>();
            _controller = new RecipeController(_mockLogger.Object);
        }

        [Fact]
        public async Task GetRecipe_ReturnsOk_WithExistingRecipe()
        {
            // Act
            IActionResult result = await _controller.GetRecipe(0);

            // Assert
            result.Should().BeOfType<OkResult>();
        }
    }
}