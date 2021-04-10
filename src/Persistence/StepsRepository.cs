using Microsoft.Extensions.Logging;

using RecipeBook.Core.Domain.Recipes;

namespace RecipeBook.Infrastructure.Persistence
{
    public class StepsRepository : RecipeResourceRepository<StepsRepository, Step, int?>
    {
        public StepsRepository(
            ILogger<StepsRepository> logger,
            string                   connectionString = "")
            : base(logger, connectionString: connectionString)
        {
        }
    }
}