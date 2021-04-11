using Microsoft.Extensions.Logging;

using RecipeBook.Core.Domain.Recipes;

namespace RecipeBook.Infrastructure.Persistence.Repositories
{
    public class StepsRepository : RepositoryBase<StepsRepository, Step, int?>
    {
        public StepsRepository(
            ILogger<StepsRepository> logger,
            string                   connectionString = "")
            : base(logger, connectionString: connectionString)
        {
        }
    }
}