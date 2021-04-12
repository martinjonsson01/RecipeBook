using System.Linq;

using RecipeBook.Core.Domain.Recipes;
using RecipeBook.Core.Domain.Units;
using RecipeBook.Presentation.WebApp.Server.Controllers.v1;

namespace Tests.WebApp.Server.Controllers
{
    public class IngredientsControllerTests
        : ResourceControllerTestBase<
            IngredientsController,
            Ingredient,
            int?>
    {
        public IngredientsControllerTests()
        {
            Controller = new IngredientsController(MockLogger.Object, MockRepo.Object);
        }

        protected override int? GetKey(Ingredient resource) => resource.Id;

        protected override int? MockKey() => 1;

        protected override int?[] MockKeys(int count) =>
            Enumerable.Range(0, count).Select(i => (int?) i).ToArray(); // array of ids from 0...count

        protected override Ingredient MockResource(string recipeName, int? key = default)
        {
            key ??= MockKey();
            return new Ingredient
            {
                Id = key,
                Name = Faker.Lorem.Sentence(),
                Amount = Faker.PickRandom<Unit>(
                    new Mass { Id = key, Value = Faker.Random.Double(10) },
                    new Volume { Id = key, Value = Faker.Random.Double(10) }
                )
            };
        }
    }
}