using Microsoft.Extensions.Logging;

using RecipeBook.Core.Domain.Recipes;

namespace RecipeBook.Infrastructure.Persistence.Repositories
{
    public class UsedOccasionsRepository
        : RepositoryBase<UsedOccasionsRepository, UsedOccasion, int?>
    {
        public UsedOccasionsRepository(
            ILogger<UsedOccasionsRepository> logger,
            string                           connectionString = "")
            : base(logger, connectionString: connectionString)
        {
        }
    }
}