using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Npgsql;

using RecipeBook.Core.Application.Repositories;
using RecipeBook.Core.Domain.Recipes;

namespace RecipeBook.Infrastructure.Persistence.Repositories
{
    public class RecipesRepository : RepositoryBase<RecipesRepository, Recipe, string>
    {
        private readonly IResourcesRepository<Step, int?>         _stepRepository;
        private readonly IResourcesRepository<Ingredient, int?>   _ingredientRepository;

        public RecipesRepository(
            ILogger<RecipesRepository>               logger,
            IResourcesRepository<Step, int?>         stepRepository,
            IResourcesRepository<Ingredient, int?>   ingredientRepository,
            string                                   connectionString = "")
            : base(logger, connectionString: connectionString)
        {
            _stepRepository = stepRepository;
            _ingredientRepository = ingredientRepository;
        }

        protected override string GetAllSql => @"
                    SELECT * 
                      FROM recipes
                  ORDER BY id; 
            ";

        protected override string GetSql => @"
                    SELECT * 
                      FROM recipes
                     WHERE name = :key; 
            ";

        protected override string ExistsSql => @"
                    SELECT EXISTS(
                        SELECT *
                          FROM recipes R
                         WHERE R.name = :key
                    ); 
            ";

        protected override string CreateOrUpdateSql(string idQuery, int recipeId, Recipe? entity) => $@"
                        INSERT
                          INTO recipes
                               (id, name, rating, imagepath)
                        VALUES ({idQuery}, :Name, :Rating, :ImagePath)
                   ON CONFLICT (id) 
                     DO UPDATE
                           SET name = :Name,
                               rating = :Rating,
                               imagepath = :ImagePath
                         WHERE recipes.id = :Id
                     RETURNING *;
                ";

        protected override string DeleteSql => @"
                DELETE 
                  FROM recipes
                 WHERE name = :key
            ";

        protected override void SetEntityKey(dynamic entity, string key) => entity.Id = int.Parse(key);

        protected override bool EntityKeyIsNull(dynamic entity) => entity.Id is null;

        public override async Task<Recipe?> CreateOrUpdateAsync(string recipeName, Recipe toStore)
        {
            await using var db = new NpgsqlConnection(ConnectionString);

            int recipeId = await GetRecipeId(recipeName, db);

            try
            {
                string idQuery = EntityKeyIsNull(toStore) ? "default" : ":Id";

                AddTypeHandlers();

                Recipe? result = await CreateOrUpdateSendQueryAsync(toStore, db, idQuery, recipeId);
                foreach (Ingredient ingredient in toStore.Ingredients)
                {
                    Ingredient? storedIngredient =
                        await _ingredientRepository.CreateOrUpdateAsync(toStore.Name, ingredient);
                    if (storedIngredient is null) continue;
                    result?.Ingredients.Add(storedIngredient);
                }
                foreach (Step step in toStore.Steps)
                {
                    Step? storedStep =
                        await _stepRepository.CreateOrUpdateAsync(toStore.Name, step);
                    if (storedStep is null) continue;
                    result?.Steps.Add(storedStep);
                }
                return result;
            }
            catch (NpgsqlException e)
            {
                if (e.SqlState != "23505") throw;

                Logger.LogWarning("Could not create or update resource because of column conflict");
                return null;
            }
        }
    }
}