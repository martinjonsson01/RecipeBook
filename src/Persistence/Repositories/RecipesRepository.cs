using Microsoft.Extensions.Logging;

using RecipeBook.Core.Domain.Recipes;

namespace RecipeBook.Infrastructure.Persistence.Repositories
{
    public class RecipesRepository : RepositoryBase<RecipesRepository, Recipe, string>
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
                     RETURNING id;
                ";

        protected override string DeleteSql => @"
                DELETE 
                  FROM recipes
                 WHERE name = :key
            ";

        protected override void SetEntityKey(dynamic entity, string key) => entity.Id = int.Parse(key);

        protected override bool EntityKeyIsNull(dynamic entity) => entity.Id is null;
        
    }
}