using System.Threading.Tasks;

using Dapper;

using Microsoft.Extensions.Logging;

using Npgsql;

using RecipeBook.Core.Domain.Recipes;

namespace RecipeBook.Infrastructure.Persistence
{
    public class RecipesRepository : RecipeResourceRepository<RecipesRepository, Recipe, string>
    {
        public RecipesRepository(ILogger<RecipesRepository> logger, string connectionString = "")
            : base(logger, connectionString: connectionString)
        {
        }

        protected override string GetAllSql => @"
                    SELECT * 
                      FROM recipes; 
            ";

        protected override string GetSql => @"
                    SELECT * 
                      FROM recipes
                     WHERE name = :key; 
            ";

        public override async Task<bool> ExistsAsync(string _, string recipeName)
        {
            await using var db = new NpgsqlConnection(ConnectionString);

            return await db.QuerySingleAsync<bool>(@"
                    SELECT EXISTS(
                        SELECT *
                          FROM recipes R
                         WHERE R.name = :recipeName
                    ); 
            ", new { recipeName });
        }

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
                     RETURNING id;
                ";
        
        protected override string? GetEntityKey(dynamic entity) => entity.Name;

        protected override void SetEntityKey(dynamic entity, string key) => entity.Id = int.Parse(key);

        protected override bool EntityKeyIsNull(dynamic entity) => entity.Id is null;
        
        public override async Task DeleteAsync(string _, string recipeName)
        {
            await using var db = new NpgsqlConnection(ConnectionString);

            await db.ExecuteAsync(@"
                DELETE 
                  FROM recipes
                 WHERE name = :recipeName
            ", new { recipeName });
        }
    }
}