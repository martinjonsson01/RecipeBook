using Dapper;

using Microsoft.Extensions.Logging;

using RecipeBook.Core.Domain.Recipes;

namespace RecipeBook.Infrastructure.Persistence.Repositories
{
    public class IngredientsRepository : RepositoryBase<IngredientsRepository, Ingredient, int?>
    {
        public IngredientsRepository(
            ILogger<IngredientsRepository> logger,
            string                         connectionString = "")
            : base(logger, connectionString: connectionString)
        {
            ExcludedPropertyNames.Add(nameof(Ingredient.Amount));
            LoadEntityProperties();
        }

        protected override void AddTypeHandlers()
        {
            SqlMapper.AddTypeHandler(new UnitTypeHandler());
        }
    }
}