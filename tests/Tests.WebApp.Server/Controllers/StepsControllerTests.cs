using System.Linq;

using RecipeBook.Core.Domain.Recipes;
using RecipeBook.Presentation.WebApp.Server.Controllers.v1;

using Tests.Shared;

namespace Tests.WebApp.Server.Controllers
{
    public class StepsControllerTests
        : ResourceControllerTestBase<
            StepsController,
            Step,
            int?>
    {
        public StepsControllerTests()
        {
            Controller = new StepsController(MockLogger.Object, MockRepo.Object);
        }

        protected override int? GetKey(Step resource) => resource.Id;

        protected override int? MockKey() => 1;

        protected override int?[] MockKeys(int count) =>
            Enumerable.Range(0, count).Select(i => (int?) i).ToArray(); // array of ids from 0...count

        protected override Step MockResource(string recipeName, int? key = default)
        {
            Step step = Fakers.Step.Generate();
            step.Id = key ?? MockKey();
            return step;
        }
    }
}