using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

using RecipeBook.Core.Domain.Recipes;
using RecipeBook.Presentation.WebApp.Server.Controllers.v1;

using Xunit;

namespace Tests.WebApp.Server.Controllers
{
    public class RecipesControllerTests
        : ResourceControllerTestBase<
            ResourceController<RecipesController, Recipe, string>,
            RecipesController,
            Recipe,
            string>
    {
        public RecipesControllerTests()
        {
            Controller = new RecipesController(MockLogger.Object, MockRepo.Object);
        }

        protected override string GetKey(Recipe resource) => resource.Name;

        protected override string MockKey() => Faker.Lorem.Sentence();

        protected override string[] MockKeys(int count) =>
            Enumerable.Range(0, count).Select(_ => MockKey()).ToArray();

        protected override Recipe MockResource(string recipeName, string? key = default)
        {
            return new()
            {
                Name = key ?? MockKey(),
                Rating = Faker.Random.Number(1, 10),
                ImagePath = Faker.Internet.Avatar()
            };
        }

        [Fact]
        public async Task Get_WithUrlSafeName_ReturnsRecipe()
        {
            // Arrange
            Recipe recipe      = MockResourceInRepository(string.Empty, Faker.Lorem.Sentence());
            string urlSafeName = recipe.ToUrlSafeName();
            
            // Act
            ActionResult<Recipe> response = await Controller.Get(string.Empty, urlSafeName);

            // Assert
            response.Result.Should().BeAssignableTo<ObjectResult>();
            var objectResult = (ObjectResult) response.Result;

            objectResult.Value.Should().BeOfType<Recipe>();
            var retrievedResource = (Recipe) objectResult.Value;

            retrievedResource.Should().BeEquivalentTo(recipe);
        }
    }
}