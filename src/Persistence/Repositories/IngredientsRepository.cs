using System;
using System.Collections.Generic;
using System.Data;
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

        protected override void AddTypeHandlers()
        {
            SqlMapper.AddTypeHandler(new UnitTypeHandler());
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
    }
}