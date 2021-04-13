using System.Linq;

using RecipeBook.Core.Domain.Recipes;
using RecipeBook.Presentation.WebApp.Server.Controllers.v1;

using Tests.Shared;

namespace Tests.WebApp.Server.Controllers
{
    public class UsedOccasionsControllerTests
        : ResourceControllerTestBase<
            UsedOccasionsController,
            UsedOccasion,
            int?>
    {
        public UsedOccasionsControllerTests()
        {
            Controller = new UsedOccasionsController(MockLogger.Object, MockRepo.Object);
        }

        protected override int? GetKey(UsedOccasion resource) => resource.Id;

        protected override int? MockKey() => 1;

        protected override int?[] MockKeys(int count) => 
            Enumerable.Range(0, count).Select(i => (int?) i).ToArray(); // array of ids from 0...count

        protected override UsedOccasion MockResource(string recipeName, int? key = default)
        {
            UsedOccasion usedOccasion = Fakers.UsedOccasion.Generate();
            usedOccasion.Id = key ?? MockKey();
            return usedOccasion;
        }
    }
}