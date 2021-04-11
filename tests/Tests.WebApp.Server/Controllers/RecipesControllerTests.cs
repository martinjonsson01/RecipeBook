using System.Linq;

using RecipeBook.Core.Domain.Recipes;
using RecipeBook.Presentation.WebApp.Server.Controllers.v1;

namespace Tests.WebApp.Server.Controllers
{
    public class RecipesControllerTests
        : RecipeResourceControllerTestBase<
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
    }
}