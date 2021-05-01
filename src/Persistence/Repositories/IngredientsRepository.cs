using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using Microsoft.Extensions.Logging;

using Npgsql;

using RecipeBook.Core.Domain.Recipes;
using RecipeBook.Core.Domain.Units;

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

        protected override string GetAllSql => @"
                    SELECT ingredients.id as IngredientId, ingredients.name as IngredientName,
                           masses.id as MassId,
                           volumes.id as VolumeId,
                           units.value as Value
                      FROM ingredients
                 LEFT JOIN units on ingredients.id = units.id
                 LEFT JOIN masses on units.id = masses.id
                 LEFT JOIN volumes on units.id = volumes.id
                     WHERE recipeid = :recipeId;
            ";

        protected override string GetSql => @"
                    SELECT ingredients.id as IngredientId, ingredients.name as IngredientName,
                           masses.id as MassId,
                           volumes.id as VolumeId,
                           units.value as Value
                      FROM ingredients
                 LEFT JOIN units on ingredients.id = units.id
                 LEFT JOIN masses on units.id = masses.id
                 LEFT JOIN volumes on units.id = volumes.id
                     WHERE ingredients.id = :key
                       AND recipeid = :recipeId;
            ";

        protected override string CreateOrUpdateSql(string idQuery, int recipeId, Ingredient ingredient) => $@"
            WITH {(idQuery != "default" ? @"
            delete_volume AS (
              DELETE FROM volumes
                WHERE id = :Id
            ),
            delete_mass AS (
              DELETE FROM masses
                WHERE id = :Id
            )," : "")}
            insert_ingredient AS (
               INSERT INTO ingredients (id, name, recipeid)
               VALUES ({idQuery}, :Name, {recipeId})
               ON CONFLICT (id)
                 DO UPDATE
                       SET name = :Name
                     WHERE ingredients.id = {(idQuery == "default" ? "-1" : idQuery)}
               RETURNING id AS ingredient_id, name AS ingredient_name
            ),
            insert_unit AS (
               INSERT INTO units (id, value)
               SELECT ingredient_id, {ingredient.Amount.Value.ToString(CultureInfo.InvariantCulture)} FROM insert_ingredient
               ON CONFLICT (id)
                 DO UPDATE
                       SET value = {ingredient.Amount.Value.ToString(CultureInfo.InvariantCulture)}
                     WHERE units.id = (SELECT ingredient_id FROM insert_ingredient)
               RETURNING id AS unit_id, value AS unit_value
            )
            INSERT INTO {(ingredient.Amount is Mass ? "masses" : "volumes")} (id)
            SELECT unit_id FROM insert_unit
            ON CONFLICT (id)
              DO UPDATE
                    SET id = (SELECT unit_id FROM insert_unit)
                  WHERE {(ingredient.Amount is Mass ? "masses" : "volumes")}.id = (SELECT ingredient_id FROM insert_ingredient)
            RETURNING
                (SELECT ingredient_id AS IngredientId FROM insert_ingredient),
                (SELECT ingredient_name as IngredientName FROM insert_ingredient),
                (SELECT unit_id AS {(ingredient.Amount is Mass ? "MassId" : "VolumeId")} FROM insert_unit),
                (SELECT NULL AS {(ingredient.Amount is Mass ? "VolumeId" : "MassId")} FROM insert_unit),
                (SELECT unit_value as Value FROM insert_unit);
            ";

        public override async Task<IEnumerable<Ingredient>> GetAllAsync(string recipeName)
        {
            await using var db = new NpgsqlConnection(ConnectionString);

            int recipeId = await GetRecipeId(recipeName, db);

            IEnumerable<dynamic> results = await db.QueryAsync(GetAllSql, new { recipeId });

            return results.Select(Ingredient.MapFromRow)
                          .ToList();
        }

        public override async Task<Ingredient?> GetAsync(string recipeName, int? key)
        {
            await using var db = new NpgsqlConnection(ConnectionString);

            int recipeId = await GetRecipeId(recipeName, db);

            IEnumerable<dynamic> results = await db.QueryAsync(GetSql, new { key, recipeId });

            return results.Select(Ingredient.MapFromRow)
                          .FirstOrDefault();
        }

        protected override async Task<Ingredient?> CreateOrUpdateSendQueryAsync(Ingredient entity, NpgsqlConnection? db, string idQuery, int recipeId)
        {
            string sql = CreateOrUpdateSql(idQuery, recipeId, entity);
            var insertedEntity = await db.QuerySingleAsync<dynamic>(
                sql,
                entity);

            return Ingredient.MapFromRow(insertedEntity);
        }
    }
}